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

        // path on android: "content://media/picker_get_content/0/com.android.providers.media.photopicker/media/38"
        
        return files;
    }
}
