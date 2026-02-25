import { Injectable, signal } from '@angular/core';
import { NoticeDialogState } from '../models/dialog-states/notice-dialog-state';

@Injectable({
    providedIn: 'root',
})
export class DialogService {
    isShowLoadingDialog = signal<boolean>(false);

    noticeDialogState = signal<NoticeDialogState>({
        isShow: false,
        isSuccess: true,
        message: '',
        title: '',
    });

    updateNoticeDialogState(
        isShow: boolean | null = null,
        isSuccess: boolean | null = null,
        message: string | null = null,
        title: string | null = null
    ) {
        this.noticeDialogState.update((x) => ({
            isShow: isShow === null ? x.isShow : isShow,
            isSuccess: isSuccess === null ? x.isSuccess : isSuccess,
            message: message === null ? x.message : message,
            title: title === null ? x.title : title,
        }));
    }

    showLoadingDialog(flag: boolean) {
        this.isShowLoadingDialog.set(flag);
    }
}
