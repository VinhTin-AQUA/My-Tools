import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { DialogService } from '../../../core/services/dialog-service';
import { ToastStore } from '../../stores/toast.store';

@Component({
    selector: 'app-toast',
    imports: [CommonModule],
    templateUrl: './toast.html',
    styleUrl: './toast.css',
})
export class Toast {
    toastStore = ToastStore;

    constructor(public dialogService: DialogService) {}

    hide() {
        this.toastStore.showToastMessage(false, true, '', '');
    }
}
