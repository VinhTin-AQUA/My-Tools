import { writeFile, BaseDirectory } from '@tauri-apps/plugin-fs';
import { AndroidFs, AndroidPublicImageDir } from 'tauri-plugin-android-fs-api';
import { platform } from '@tauri-apps/plugin-os';

// rust: https://crates.io/crates/tauri-plugin-android-fs
// js binding: https://www.npmjs.com/package/tauri-plugin-android-fs-api?activeTab=readme

export class FileSystemHelper {
    static async writeImgToPicturesFromBase64(fileName: string, base64: string) {
        const currentPlatform = platform();

        if (currentPlatform === 'android') {
            const contents = FileSystemHelper.convertBase64ToUint8Array(base64);
            await FileSystemHelper.saveImgToPicturesFromBytes(fileName, contents);
        } else {
            FileSystemHelper.saveImgToDownloadFromBase64(fileName, base64);
        }
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
