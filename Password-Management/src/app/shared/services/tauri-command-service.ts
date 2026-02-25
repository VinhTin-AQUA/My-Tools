import { inject, Injectable } from '@angular/core';
import { invoke } from '@tauri-apps/api/core';
import { DialogService } from './dialog-service';
import { SpreadsheetConfigStore } from '../stores/spread-sheet-config-store';

@Injectable({
    providedIn: 'root',
})
export class TauriCommandSerivce {
    static readonly INIT_GOOGLE_SHEET_COMMAND = 'init_google_sheet_command';
    static readonly ADD_ACCOUNT = 'add_account';
    static readonly GET_ACCOUNTS = 'get_accounts';
    static readonly DELETE_ACCOUNT = 'delete_account';
    static readonly UPDATE_ACCOUNT = 'update_account';

    spreadsheetConfigStore = inject(SpreadsheetConfigStore);

    constructor(private dialogService: DialogService) {}

    async invokeCommand<T>(cmd: string, params: any): Promise<T | null> {
        this.dialogService.showLoadingDialog(true);
        try {
            const initOk = await invoke<T>(cmd, {
                sheetName: this.spreadsheetConfigStore.workingSheet().title,
                spreadsheetId: this.spreadsheetConfigStore.spreadSheetId(),
                ...params,
            });
            this.dialogService.showLoadingDialog(false);
            return initOk;
        } catch (e) {
            alert(e);
            console.log('e: ', e);
            this.dialogService.showLoadingDialog(false);
            return null;
        }
    }
}
