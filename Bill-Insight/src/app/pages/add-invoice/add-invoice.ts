import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faArrowsRotate, faTrash, faCirclePlus } from '@fortawesome/free-solid-svg-icons';
import { TauriCommandSerivce } from '../../shared/services/tauri/tauri-command-service';
import { CaptchaSession } from './models/captcha-session';
import { AppFolderHelper } from '../../shared/helpers/app-folder';
import { EAppFolderNames } from '../../core/enums/folder-names';
import { convertFileSrc } from '@tauri-apps/api/core';
import { HHDVu, NBan, ReadXmlDataResult, TToan } from './models/xml_data';
import { CashItem, InvoiceExcel } from './models/invoice-excel';
import { formatDate } from '@angular/common';
import { DialogService } from '../../shared/services/dialog-service';
import { ResponseCommad } from '../../shared/models/response-command';
import { SpreadsheetConfigStore } from '../../shared/stores/config-store';
import { FileHelper } from '../../shared/helpers/file-helper';
import {TranslatePipe} from "@ngx-translate/core";

@Component({
    selector: 'app-add-invoice',
    imports: [FormsModule, DecimalPipe, FontAwesomeModule, TranslatePipe],
    templateUrl: './add-invoice.html',
    styleUrl: './add-invoice.scss',
})
export class AddInvoice {
    captchaInput = '';
    invoiceCode = '';
    invoiceCT = '';

    hhdvus = signal<HHDVu[]>([]);
    nban = signal<NBan>({ d_chi: '', mst: '', sdt: '', ten: '', tt_khac: [] });
    ttoan = signal<TToan>({
        t_httl_t_suat: [],
        tg_t_thue: 0,
        tg_tc_thue: 0,
        tg_tt_tb_chu: '',
        tg_tt_tb_so: 0,
        tt_khac: [],
    });

    cashItems = signal<CashItem[]>([]);
    invoiceDate = new Date().toISOString().substring(0, 10);
    spreadsheetConfigStore = inject(SpreadsheetConfigStore);

    faArrowsRotate = faArrowsRotate;
    faTrash = faTrash;
    faCirclePlus = faCirclePlus;
    captchaSession = signal<CaptchaSession>({
        aspnet_session_id: '',
        captcha_path: '',
        sv_id: '',
    });

    constructor(
        private tauriCommandSerivce: TauriCommandSerivce,
        private dialogService: DialogService
    ) {}

    ngOnInit() {}

    addInvoiceItem() {
        this.hhdvus.update((group) => [
            ...group,
            {
                cash: '',
                d_gia: 0,
                dv_tinh: '',
                id: crypto.randomUUID(),
                mhhdvu: '',
                s_luong: 0,
                t_suat: '',
                th_tien: 0,
                th_tien_sau_lai_suat: 0,
                thhdvu: '',
            },
        ]);
    }

    removeInvoiceItem(id: string) {
        const index = this.hhdvus().findIndex((item) => item.id === id);
        if (index !== -1) {
            const hhdvus = [...this.hhdvus()];
            hhdvus.splice(index, 1);
            this.hhdvus.set(hhdvus);
        }
    }

    addExtraItem() {
        this.cashItems.update((x) => [
            ...x,
            {
                cash: 0,
                id: crypto.randomUUID(),
                name: '',
            },
        ]);
    }

    removeExtraItem(id: string) {
        const index = this.cashItems().findIndex((item) => item.id === id);
        if (index !== -1) {
            const cashItems = [...this.cashItems()];
            cashItems.splice(index, 1);
            this.cashItems.set(cashItems);
        }
    }

    async saveInvoice() {
        const formattedDate = formatDate(this.invoiceDate, 'dd/MM/yyyy', 'en-US');
        const list1 = this.hhdvus().map((x: HHDVu) => ({
            bank: x.th_tien_sau_lai_suat,
            cash: 0,
            invoice_date: formattedDate,
            name: x.thhdvu,
        }));

        const list2 = this.cashItems().map((x: CashItem) => ({
            bank: 0,
            cash: x.cash,
            invoice_date: formattedDate,
            name: x.name,
        }));

        let list = [...list1, ...list2];
        list = list.map((item, index) => ({
            ...item,
            invoice_date: index === 0 ? formattedDate : '-',
        }));

        const responseCommad = await this.tauriCommandSerivce.invokeCommand<ResponseCommad>(
            TauriCommandSerivce.SET_INVOICES,
            {
                sheetName: this.spreadsheetConfigStore.workingSheet().title,
                spreadsheetId: this.spreadsheetConfigStore.spreadSheetId(),
                items: list,
            }
        );

        this.dialogService.updateNoticeDialogState(
            true,
            responseCommad?.is_success,
            responseCommad?.message,
            responseCommad?.title
        );

        const tempFolder = await AppFolderHelper.getFolderPath(EAppFolderNames.TempDir);
        await FileHelper.clearAllFilesInFolder(tempFolder)
    }

    async loadCaptcha() {
        const tempFolder = await AppFolderHelper.getFolderPath(EAppFolderNames.TempDir);
        const captcha_and_asp_session =
            await this.tauriCommandSerivce.invokeCommand<CaptchaSession>(
                TauriCommandSerivce.GET_CAPTCHA_AND_ASP_SESSION,
                { folder: tempFolder }
            );

        if (!captcha_and_asp_session) {
            return;
        }

        this.captchaSession.set({
            captcha_path: convertFileSrc(captcha_and_asp_session.captcha_path),
            aspnet_session_id: captcha_and_asp_session.aspnet_session_id,
            sv_id: captcha_and_asp_session.sv_id,
        });
    }

    async getInvoiceInfo() {
        const tempFolder = await AppFolderHelper.getFolderPath(EAppFolderNames.TempDir);
        const xml_data = await this.tauriCommandSerivce.invokeCommand<ReadXmlDataResult>(
            TauriCommandSerivce.GET_XML_INVOICE_DATA,
            {
                svId: this.captchaSession().sv_id,
                aspSession: this.captchaSession().aspnet_session_id,
                captcha: this.captchaInput,
                phone: this.invoiceCT,
                invoiceNum: this.invoiceCode,
                folder: tempFolder,
            }
        );

        if (!xml_data) {
            return;
        }

        this.hhdvus.set(xml_data.hhdvus);
        this.nban.set(xml_data.nban);
        this.ttoan.set(xml_data.ttoan);
    }

    onCashInput(event: Event, item: CashItem) {
        const input = event.target as HTMLInputElement;
        input.value = input.value.replace(/[^0-9]/g, '');
        item.cash = +input.value;
    }

    onInvoiceInput(event: Event, item: HHDVu) {
        const input = event.target as HTMLInputElement;
        input.value = input.value.replace(/[^0-9]/g, '');
        item.th_tien_sau_lai_suat = +input.value;
    }
}
