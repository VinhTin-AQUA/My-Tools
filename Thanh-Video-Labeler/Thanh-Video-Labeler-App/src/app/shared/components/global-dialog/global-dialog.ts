import { CommonModule } from '@angular/common';
import { Component, computed } from '@angular/core';
import { dialogSignal, hideGlobalDialog } from '../../signals/dialog-signal';

@Component({
    selector: 'app-global-dialog',
    imports: [CommonModule],
    templateUrl: './global-dialog.html',
    styleUrl: './global-dialog.scss',
})
export class GlobalDialog {
    dialog = computed(() => dialogSignal());

    closeDialog() {
        hideGlobalDialog();
    }
}
