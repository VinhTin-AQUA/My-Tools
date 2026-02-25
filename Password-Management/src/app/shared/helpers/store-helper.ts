import { load, Store } from '@tauri-apps/plugin-store';
import { SettingKeys } from '../../core/enums/setting-keys';

export class StoreHelper {
    readonly storeFileName = 'settings.json';
    public static store: Store | null = null;

    public static async init() {
        StoreHelper.store = await load('store.json', { autoSave: true, defaults: {} });
    }

    public static async setValue(key: SettingKeys, value: any) {
        if (!StoreHelper.store) {
            return;
        }

        await StoreHelper.store.set(key, value);
    }

    public static async getValue<T>(key: SettingKeys) {
        if (!StoreHelper.store) {
            return;
        }

        const val = await StoreHelper.store.get<T>(key);
        return val;
    }

    public static async delete(key: SettingKeys) {
        if (!StoreHelper.store) {
            return;
        }

        const val = await StoreHelper.store.delete(key);
        return val;
    }
}
