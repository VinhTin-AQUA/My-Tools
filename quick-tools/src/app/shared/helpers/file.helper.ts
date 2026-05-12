import {
    writeFile,
    BaseDirectory,
    readFile,
    exists,
    mkdir,
    readDir,
    remove,
    stat,
} from '@tauri-apps/plugin-fs';
import { join } from '@tauri-apps/api/path';
import { open } from '@tauri-apps/plugin-dialog';
import { IMAGE_EXTENSIONS } from '../../core/constants/file-extensions';
import { invoke } from '@tauri-apps/api/core';
import { Commands } from '../../core/constants/commands.enum';
import { FileMetadata } from '../../core/models/upload-image.model';
import { AndroidFs } from 'tauri-plugin-android-fs-api';
import { platform } from '@tauri-apps/plugin-os';

// rust: https://crates.io/crates/tauri-plugin-android-fs
// js binding: https://www.npmjs.com/package/tauri-plugin-android-fs-api?activeTab=readme

export interface SelectedFile {
    id: string;
    fileName: string;
    path: string;
    size: number;
    content: Uint8Array;
    downloaded: boolean;
    previewUrl: string;
}

export class FileHelper {
    static async selectMultiFiles(): Promise<SelectedFile[] | null> {
        const files = await open({
            multiple: true,
            directory: false,
            filters: [
                {
                    name: 'Image',
                    extensions: IMAGE_EXTENSIONS,
                },
            ],
        });

        if (!files) {
            return null;
        }

        const normalized = Array.isArray(files) ? files : [files];

        const results = await Promise.all(
            normalized.map(async (path) => {
                const metadata = await FileHelper.getStatOfFile(path);

                // read binary file
                const content = await readFile(path);

                return {
                    id: crypto.randomUUID().toString(),
                    path,
                    fileName: FileHelper.getFileName(path),
                    size: metadata.size,
                    content,
                    downloaded: false,
                    previewUrl: FileHelper.uint8ArrayToBlobUrl(content),
                };
            }),
        );

        return results;
    }

    static async writeImgToPicturesFromBase64(fileName: string, base64: string) {
        const currentPlatform = platform();

        if (currentPlatform === 'android') {
            const contents = FileHelper.convertBase64ToUint8Array(base64);
            await FileHelper.saveImgToPicturesFromBytes(fileName, contents);
        } else {
            FileHelper.saveImgToDownloadFromBase64(fileName, base64);
        }
    }

    static fileToBase64(file: File): Promise<string> {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();

            reader.onload = () => resolve(reader.result as string);
            reader.onerror = (err) => reject(err);

            reader.readAsDataURL(file);
        });
    }

    static async saveFileToAppData(
        fileName: string,
        appFolder: string,
        content: string | Uint8Array,
    ): Promise<string> {
        // Windows: C:\Users\<user>\AppData\Roaming\com.example.myapp\data\config.json
        // macOS: ~/Library/Application Support/com.example.myapp/data/config.json
        // Android: /data/data/com.example.myapp/files/data/config.json

        const folderExists = await exists(appFolder, {
            baseDir: BaseDirectory.AppData,
        });

        if (!folderExists) {
            await mkdir(appFolder, {
                baseDir: BaseDirectory.AppData,
                recursive: true,
            });
        }

        const filePath = `${appFolder}/${fileName}`;
        const data = typeof content === 'string' ? new TextEncoder().encode(content) : content;

        await writeFile(filePath, data, {
            baseDir: BaseDirectory.AppData,
        });

        const fullPath = await join(String(BaseDirectory.AppData), appFolder, fileName);
        return fullPath;
    }

    static async saveFilesToAppData(files: SelectedFile[], appFolder: string): Promise<string[]> {
        const savedPaths: string[] = [];

        for (const file of files) {
            const savedPath = await this.saveFileToAppData(file.fileName, appFolder, file.content);

            savedPaths.push(savedPath);
        }

        return savedPaths;
    }

    static async clearScaledFolderInAppData(folderName: string) {
        const entries = await readDir(folderName, {
            baseDir: BaseDirectory.AppData,
        });

        // xóa từng file
        for (const entry of entries) {
            if (entry.isFile && entry.name) {
                await remove(`${folderName}/${entry.name}`, {
                    baseDir: BaseDirectory.AppData,
                });
            }
        }
    }

    static async getStatOfFile(path: string) {
        const info = await stat(path, {
            baseDir: BaseDirectory.AppData,
        });
        return info;
    }

    static getFileName(path: string): string {
        return path.split(/[\\/]/).pop() ?? '';
    }

    //----------------------------------------------------------

    static uint8ArrayToBlobUrl(data: Uint8Array, mimeType = 'image/png'): string {
        // clone sang buffer chuẩn ArrayBuffer
        const safeArray = new Uint8Array(data);

        const blob = new Blob([safeArray], {
            type: mimeType,
        });

        return URL.createObjectURL(blob);
    }

    private static async saveImgToPicturesFromBytes(fileName: string, contents: Uint8Array) {
        const baseDir = 'Pictures';
        const relativePath = fileName;
        const mimeType = 'image/png';

        const uri = await AndroidFs.createNewPublicImageFile(baseDir, relativePath, mimeType);

        try {
            const path = await AndroidFs.getFsPath(uri);
            await writeFile(path, contents, {
                baseDir: BaseDirectory.Picture,
            });

            await AndroidFs.scanPublicFile(uri);
        } catch (e) {
            await AndroidFs.removeFile(uri).catch(() => {});
            throw e;
        }
    }

    private static saveImgToDownloadFromBase64(fileName: string, base64: string) {
        const link = document.createElement('a');
        link.href = base64; // dạng data:image/png;base64,...
        link.download = fileName;
        link.click();
    }

    private static convertBase64ToUint8Array(base64: string) {
        // Loại bỏ tiền tố "data:image/...;base64,"
        const cleanedBase64 = base64.replace(/^data:.*;base64,/, '');

        // Giải mã Base64 thành chuỗi nhị phân
        const binaryString = atob(cleanedBase64);

        // Tạo Uint8Array từ chuỗi nhị phân
        const len = binaryString.length;
        const bytes = new Uint8Array(len);

        for (let i = 0; i < len; i++) {
            bytes[i] = binaryString.charCodeAt(i);
        }
        return bytes;
    }
}
