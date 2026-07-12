import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { TauriCommandService } from '../../core/services/tauri-command-service';
import { ToastStore } from '../../shared/stores/toast.store';
import { DialogHelper } from '../../shared/helpers/dialog-helper.helper';
import { QuestionCancelDialog } from '../../shared/components/question-cancel-dialog/question-cancel-dialog';

@Component({
    selector: 'app-settings',
    imports: [CommonModule, TranslatePipe, QuestionCancelDialog],
    templateUrl: './settings.html',
    styleUrl: './settings.css',
})
export class Settings {
    importFile!: File | null;
    isShowQuestionDialog = signal<boolean>(false);

    constructor(private tauriCommandService: TauriCommandService) {}

    onImportFile(event: any) {
        this.importFile = event.target.files[0] ?? null;
    }

    async importData() {
        const path = await DialogHelper.openFileSelectorDialog();
        if (!path) {
            return;
        }

        const r = await this.tauriCommandService.invokeCommand<boolean>(
            TauriCommandService.IMPORT_JSON_TO_DB_COMMAND,
            { path: path }
        );

        if (!r) {
            ToastStore.showToastMessage(true, false, 'Error', 'Có lỗi import dữ liệu');
            return;
        }
        ToastStore.showToastMessage(true, true, 'Success', 'Import dữ liệu thành công');
    }

    async exportData() {
        const r = await this.tauriCommandService.invokeCommand<boolean>(
            TauriCommandService.EXPOT_JSON_DATA_COMMAND,
            {}
        );
        if (!r) {
            ToastStore.showToastMessage(true, false, 'Error', 'Có lỗi export dữ liệu');
            return;
        }
        ToastStore.showToastMessage(true, true, 'Success', 'Export dữ liệu thành công');
    }

    onShowDelete() {
        this.isShowQuestionDialog.set(true);
    }

    async onShowQuestionDialogClose(result: boolean) {
        this.isShowQuestionDialog.set(false);

        if (!result) {
            return;
        }

        const r = await this.tauriCommandService.invokeCommand<boolean>(
            TauriCommandService.DELETE_ALL_DATA_COMMAND,
            {}
        );

        if (!r) {
            ToastStore.showToastMessage(true, false, 'Error', 'Có lỗi xóa dữ liệu');
            return;
        }
        ToastStore.showToastMessage(true, true, 'Success', 'Xóa tất cả dữ liệu thành công');
    }

    resetAll() {}
}
