import { writeFile, BaseDirectory, readFile } from '@tauri-apps/plugin-fs';
import { AndroidFs, AndroidPublicImageDir } from 'tauri-plugin-android-fs-api';
import { platform } from '@tauri-apps/plugin-os';
import { convertFileSrc } from '@tauri-apps/api/core';

// rust: https://crates.io/crates/tauri-plugin-android-fs
// js binding: https://www.npmjs.com/package/tauri-plugin-android-fs-api?activeTab=readme

export class FileHelper {
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

    static async fileToBlobUrl(path: string) {

        const t = convertFileSrc(path)

        const data = await readFile(path);
        const blob = new Blob([data]);
        return URL.createObjectURL(blob);
    }

    //----------------------------------------------------------

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
