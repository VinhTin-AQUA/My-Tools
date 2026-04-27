import {
    isPermissionGranted,
    requestPermission,
    sendNotification,
} from '@tauri-apps/plugin-notification';

export class NotificationHelper {
    static async requestPermission() {
        // Do you have permission to send a notification?
        let permissionGranted = await isPermissionGranted();

        // If not we need to request it
        if (!permissionGranted) {
            const permission = await requestPermission();
            permissionGranted = permission === 'granted';
        }
    }

    static async sendNotification(title: string, body: string) {
        let permissionGranted = await isPermissionGranted();

        if (permissionGranted) {
            console.log(123);
            
            sendNotification({ title: title, body: body });
        }
    }
}
