import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { DialogService } from '../../../services/dialog-service';

@Component({
    selector: 'app-notice-dialog',
    imports: [CommonModule],
    templateUrl: './notice-dialog.html',
    styleUrl: './notice-dialog.scss',
})
export class NoticeDialog {
    constructor(public dialogService: DialogService) {}

    closeDialog() {
        this.dialogService.updateNoticeDialogState(false)
    }
}
