import { inject, Injectable, signal } from '@angular/core';
import { ToastMessageStore } from '../stores/toast-message.store';

@Injectable({
    providedIn: 'root',
})
export class DialogService {
    isShowLoadingDialog = signal<boolean>(false);
    isShowToastMessage = signal<boolean>(false);
    private timeoutId: ReturnType<typeof setTimeout> | null = null;

    private toastMessageStore = inject(ToastMessageStore);

    showLoadingDialog(flag: boolean) {
        this.isShowLoadingDialog.set(flag);
    }

    showToastMessage(flag: boolean, title: string, message: string, isSuccess: boolean) {
        this.toastMessageStore.update({ title, message, isSuccess });
        this.isShowToastMessage.set(flag);

        if (flag === true) {
            this.timeoutId = setTimeout(() => this.isShowToastMessage.set(false), 3500);
        } else {
            if (this.timeoutId) {
                clearTimeout(this.timeoutId);
                this.timeoutId = null;
            }
        }
    }
}
