import { Component, computed } from '@angular/core';
import { globalLoadingSignal } from '../../signals/loading-signal';

@Component({
    selector: 'app-global-loading',
    imports: [],
    templateUrl: './global-loading.html',
    styleUrl: './global-loading.scss',
})
export class GlobalLoading {
    isLoading = computed(() => globalLoadingSignal());
}
