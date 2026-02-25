import { readTextFile, BaseDirectory, readDir, remove } from '@tauri-apps/plugin-fs';

export class FileHelper {
    static async getObjectFromFile<T>(filePath: string): Promise<T | null> {
        try {
            const jsonString = await readTextFile(filePath);
            const data = JSON.parse(jsonString) as T;
            return data;
        } catch {
            return null;
        }
    }

    static async clearAllFilesInFolder(folder: string) {
        try {
            const files = await readDir(folder);
            for (const file of files) {
                if (file.isFile) {
                    await remove(`${folder}/${file.name}`);
                } else if (file.isDirectory) {
                    await remove(`${folder}/${file.name}`, {
                        recursive: true,
                    });
                }
            }
        } catch (err) {}
    }
}
