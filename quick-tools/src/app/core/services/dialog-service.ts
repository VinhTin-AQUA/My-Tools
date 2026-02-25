import { inject, Injectable, signal } from '@angular/core';
import { QuestionCancelDialogStore } from '../../shared/stores/question-cancel-dialog.store';
import { ToastMessageStore } from '../../shared/stores/toast.store';

@Injectable({
    providedIn: 'root',
})
export class DialogService {
    isShowLoadingDialog = signal<boolean>(false);
    isShowQuestionCancelDialog = signal<boolean>(false);

    questionCancelDialogStore = inject(QuestionCancelDialogStore);

    isShowToastMessage = signal<boolean>(false);
    private timeoutId: ReturnType<typeof setTimeout> | null = null;
    private toastMessageStore = inject(ToastMessageStore);

    showLoadingDialog(flag: boolean) {
        this.isShowLoadingDialog.set(flag);
    }

    showQuestionCancelDialog(show: boolean, title: string, message: string, isSuccess: boolean) {
        this.isShowQuestionCancelDialog.set(show);
        this.questionCancelDialogStore.update(title, message, isSuccess);
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
