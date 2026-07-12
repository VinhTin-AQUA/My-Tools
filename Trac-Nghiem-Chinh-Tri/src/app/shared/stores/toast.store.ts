import { signal } from '@angular/core';

export class ToastStore {
    static isShow = signal<boolean>(false);
    static isSuccess = signal<boolean>(false);
    static title = signal<string>('');
    static message = signal<string>('');
    private static timeoutId: ReturnType<typeof setTimeout> | null = null;

    static showToastMessage(isShow: boolean, isSuccess: boolean, title: string, message: string) {
        this.isShow.set(isShow);
        this.isSuccess.set(isSuccess);
        this.title.set(title);
        this.message.set(message);

        if (isShow === true) {
            this.timeoutId = setTimeout(() => this.isShow.set(false), 3500);
        } else {
            if (this.timeoutId) {
                clearTimeout(this.timeoutId);
                this.timeoutId = null;
            }
        }
    }
}
