import { Component, inject, signal } from '@angular/core';
import { InvoiceItem, ListInvoiceItems } from './models/invoice-item';
import { DecimalPipe } from '@angular/common';
import { TauriCommandSerivce } from '../../shared/services/tauri/tauri-command-service';
import { SpreadsheetConfigStore } from '../../shared/stores/config-store';
import {TranslatePipe} from "@ngx-translate/core";

@Component({
    selector: 'app-list-invoices',
    imports: [DecimalPipe, TranslatePipe],
    templateUrl: './list-invoices.html',
    styleUrl: './list-invoices.scss',
})
export class ListInvoices {
    productGroups = signal<ListInvoiceItems[]>([]);
    spreadsheetConfigStore = inject(SpreadsheetConfigStore);

    constructor(private tauriCommandSerivce: TauriCommandSerivce) {}

    ngOnInit() {
        this.getInvoices();
    }

    async getInvoices() {
        const r = await this.tauriCommandSerivce.invokeCommand<ListInvoiceItems[]>(
            TauriCommandSerivce.GET_INVOICES,
            {
                sheetName: this.spreadsheetConfigStore.workingSheet().title,
                spreadsheetId: this.spreadsheetConfigStore.spreadSheetId(),
            }
        );

        if (r) {
            this.productGroups.set([...r]);
        } else {
            alert('null');
        }
    }

    getTotalCash(products: InvoiceItem[]) {
        return products.reduce((sum, p) => sum + p.cash_price, 0);
    }

    getTotalBank(products: InvoiceItem[]) {
        return products.reduce((sum, p) => sum + p.bank_price, 0);
    }

    toggleGroup(group: any) {
        group.expanded = !group.expanded;
    }
}
