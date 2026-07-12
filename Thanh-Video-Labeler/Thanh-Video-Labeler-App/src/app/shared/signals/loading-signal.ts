import { signal } from '@angular/core';

export const globalLoadingSignal = signal(false);

export function showGlobalLoading() {
  globalLoadingSignal.set(true);
}

export function hideGlobalLoading() {
  globalLoadingSignal.set(false);
}
