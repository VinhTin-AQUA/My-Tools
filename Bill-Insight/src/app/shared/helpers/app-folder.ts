import { appLocalDataDir, join } from '@tauri-apps/api/path';
import { exists, BaseDirectory, mkdir } from '@tauri-apps/plugin-fs';
import { EAppFolderNames } from '../../core/enums/folder-names';



export class AppFolderHelper {
    private static dataDir: string = '';

    static async getFolderPath(folder: EAppFolderNames) {
        const dataDir = await this.getDataDir();

        if (folder === EAppFolderNames.DataDir) {
            return dataDir;
        }

        const configDirExists = await exists(folder, {
            baseDir: BaseDirectory.AppLocalData,
        });
        
        if (!configDirExists) {
            await mkdir(folder, {
                baseDir: BaseDirectory.AppLocalData,
            });
        }

        let folderPath = await join(dataDir, folder);
        return folderPath;
    }

    private static async getDataDir() {
        if (this.dataDir) {
            return this.dataDir;
        }
        this.dataDir = await appLocalDataDir();
        return this.dataDir;
    }
}
