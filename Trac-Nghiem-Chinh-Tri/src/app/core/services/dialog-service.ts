import { inject, Injectable, signal } from '@angular/core';

@Injectable({
    providedIn: 'root',
})
export class DialogService {
    isShowLoadingDialog = signal<boolean>(false);

    showLoadingDialog(flag: boolean) {
        this.isShowLoadingDialog.set(flag);
    }
}
