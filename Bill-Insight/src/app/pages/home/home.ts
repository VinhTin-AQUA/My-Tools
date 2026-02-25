import { Component, inject, signal } from '@angular/core';
import { SheetStats as SheetStatsModel } from './models/sheet-stats';
import { TauriCommandSerivce } from '../../shared/services/tauri/tauri-command-service';
import { SpreadsheetConfigStore } from '../../shared/stores/config-store';
import { DecimalPipe } from '@angular/common';
import {TranslatePipe} from "@ngx-translate/core";

@Component({
    selector: 'app-home',
    imports: [DecimalPipe, TranslatePipe],
    templateUrl: './home.html',
    styleUrl: './home.scss',
})
export class Home {
    sheetStats = signal<SheetStatsModel>({
        used_cash: 0,
        used_bank: 0,
        total_cash: 0,
        total_bank: 0, 
        remaining_cash: 0,
        remaining_bank: 0,
        total_remaining: 0,
    });
    isReady = signal<boolean>(false);
    spreadsheetConfigStore = inject(SpreadsheetConfigStore);

    constructor(
        private tauriCommandSerivce: TauriCommandSerivce,
    ) {}

    ngOnInit() {
        this.getInvoices();
    }

    async getInvoices() {
        const r = await this.tauriCommandSerivce.invokeCommand<SheetStatsModel>(
            TauriCommandSerivce.GET_SHEET_STATS,
            {
                sheetName: this.spreadsheetConfigStore.workingSheet().title,
                spreadsheetId: this.spreadsheetConfigStore.spreadSheetId(),
            }
        );

        if (r) {
            this.isReady.set(true);
            this.sheetStats.set(r);
        }
    }
}
