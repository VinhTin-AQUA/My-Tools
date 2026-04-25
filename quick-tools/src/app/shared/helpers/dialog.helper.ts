import { open } from '@tauri-apps/plugin-dialog';

export class DialogHelper {
    static async selectMultiFiles() {
        // Open a dialog
        const files = await open({
            multiple: true,
            directory: false,
            filters: [{
                name:"Image",
                extensions: ["png", "jpg", "webp"]
            }]
        });
        console.log(files);
    }
}
