import { signal } from '@angular/core';

export interface DialogData {
  title: string;
  message: string;
  isSuccess: boolean;
  visible: boolean;
}

export const dialogSignal = signal<DialogData>({
  title: '',
  message: '',
  isSuccess: true,
  visible: false
});

export function showGlobalDialog(
  title: string,
  message: string,
  isSuccess: boolean
) {

  dialogSignal.set({
    title,
    message,
    isSuccess: isSuccess,
    visible: true,
  });
}

export function hideGlobalDialog() {
  dialogSignal.set({
    ...dialogSignal(),
    visible: false,
  });
}
