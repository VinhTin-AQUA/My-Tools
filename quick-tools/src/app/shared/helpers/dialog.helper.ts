import { open } from '@tauri-apps/plugin-dialog';
import { IMAGE_EXTENSIONS } from '../../core/constants/file-extensions';

export class DialogHelper {
    static async selectMultiFiles() {
        // Open a dialog
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
        return files;
    }
}
