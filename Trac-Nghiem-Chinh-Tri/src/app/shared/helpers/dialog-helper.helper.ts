import { open } from '@tauri-apps/plugin-dialog';

export class DialogHelper {
    public static async openFileSelectorDialog() {
        const file = await open({
            multiple: false,
            directory: false,
        });
        return file;
    }
}
