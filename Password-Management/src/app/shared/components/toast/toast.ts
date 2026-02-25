import { Component, inject } from '@angular/core';
import { DialogService } from '../../services/dialog-service';
import { CommonModule } from '@angular/common';
import { ToastMessageStore } from '../../stores/toast-message.store';
import { Icon } from '../icon/icon';

@Component({
    selector: 'app-toast',
    imports: [CommonModule, Icon],
    templateUrl: './toast.html',
    styleUrl: './toast.scss',
})
export class Toast {
    toastMessageStore = inject(ToastMessageStore);

    constructor(private dialogService: DialogService) {}

    hide() {
        this.dialogService.showToastMessage(false, '', '', true);
    }
}
