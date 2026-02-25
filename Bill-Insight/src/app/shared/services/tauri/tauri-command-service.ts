import { Injectable } from '@angular/core';
import { invoke } from '@tauri-apps/api/core';
import { DialogService } from '../dialog-service';

@Injectable({
    providedIn: 'root',
})
export class TauriCommandSerivce {
    static readonly INIT_GOOGLE_SHEET_COMMAND = 'init_google_sheet_command';
    static readonly GET_INVOICES = 'get_invoices';
    static readonly GET_SHEET_STATS = 'get_sheet_stats';
    static readonly GET_CAPTCHA_AND_ASP_SESSION = 'get_captcha_and_asp_session';
    static readonly GET_XML_INVOICE_DATA = 'get_xml_invoice_data';
    static readonly SET_INVOICES = 'set_invoices';
    static readonly LIST_SHEETS = 'list_sheets';
    static readonly UPDATE_SHEET_NAME = 'update_sheet_name';

    constructor(private dialogService: DialogService) {}

    async invokeCommand<T>(cmd: string, params: any): Promise<T | null> {
        this.dialogService.showLoadingDialog(true);
        try {
            const initOk = await invoke<T>(cmd, params);
            this.dialogService.showLoadingDialog(false);
            return initOk;
        } catch (e) {
            alert(e);
            console.log("e: ", e);
            this.dialogService.showLoadingDialog(false);
            return null;
        }
    }
}
