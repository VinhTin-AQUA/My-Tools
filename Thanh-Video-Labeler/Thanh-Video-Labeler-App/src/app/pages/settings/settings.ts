import { Component, signal } from '@angular/core';
import { SettingService } from './services/setting-service';
import { showGlobalDialog } from '../../shared/signals/dialog-signal';
import {
    hideGlobalLoading,
    showGlobalLoading,
} from '../../shared/signals/loading-signal';
import { FormsModule } from '@angular/forms';

@Component({
    selector: 'app-settings',
    imports: [FormsModule],
    templateUrl: './settings.html',
    styleUrl: './settings.scss',
})
export class Settings {
    excelFileName = signal<string>('video_123.mp4');
    sheetName = signal<string>('data_report.json');

    isRemoveExcefile = signal<boolean>(false);
    isRemoveDb = signal<boolean>(false);
    isRemoveVideosAndTxt = signal<boolean>(false);
    openModal = false;

    constructor(private settingService: SettingService) {}

    ngOnInit() {
        this.getSettingInfo();
    }

    getSettingInfo() {
        showGlobalLoading();
        this.settingService.getSettingInfo().subscribe({
            next: (res: any) => {
                this.excelFileName.update((x) => res.data.fileName);
                this.sheetName.update((x) => res.data.sheetName);
                hideGlobalLoading();
            },
            error: (err) => {
                showGlobalDialog('Setting Info', err.error.message, false);
                hideGlobalLoading();
            },
        });
    }

    resetData() {
        showGlobalLoading();
        this.settingService.resetData().subscribe({
            next: (res: any) => {
                this.excelFileName.update((x) => 'Nothing');
                this.sheetName.update((x) => 'Nothing');
                showGlobalDialog('Reset data', res.message, true);
                hideGlobalLoading();
            },
            error: (err) => {
                showGlobalDialog('Reset data', err.error.message, false);
                hideGlobalLoading();
            },
        });
    }
}
