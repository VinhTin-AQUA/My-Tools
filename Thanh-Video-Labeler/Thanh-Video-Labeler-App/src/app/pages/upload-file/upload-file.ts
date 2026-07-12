import { Component, inject, signal } from '@angular/core';
import { UploadService } from './upload-service';
import {
    globalLoadingSignal,
    hideGlobalLoading,
    showGlobalLoading,
} from '../../shared/signals/loading-signal';
import { showGlobalDialog } from '../../shared/signals/dialog-signal';

@Component({
    selector: 'app-upload-file',
    imports: [],
    templateUrl: './upload-file.html',
    styleUrl: './upload-file.scss',
})
export class UploadFile {
    selectedFile: File | null = null;
    uploadService = inject(UploadService);
    isAccepted = signal<boolean>(false);

    onFileSelected(event: any, fileInput: HTMLInputElement): void {
        const file = event.target.files?.[0];
        if (file) {
            const isExcel =
                file.name.endsWith('.xls') || file.name.endsWith('.xlsx');
            if (!isExcel) {
                showGlobalDialog('Upload', 'Excel is invalid', false);
                return;
            }
            this.selectedFile = file;
            this.uploadFile(false);
        }
        fileInput.value = '';
    }

    onDragOver(event: DragEvent): void {
        event.preventDefault();
    }

    onDrop(event: DragEvent): void {
        event.preventDefault();
        const file = event.dataTransfer?.files?.[0];
        if (file) {
            const isExcel =
                file.name.endsWith('.xls') || file.name.endsWith('.xlsx');
            if (!isExcel) {
                showGlobalDialog('Upload', 'Excel is invalid', false);
                return;
            }

            this.selectedFile = file;
            this.uploadFile(false);
        }
    }

    uploadFile(isAccepted: boolean) {
        showGlobalLoading();
        if (this.selectedFile) {
            this.uploadService.upload(this.selectedFile, isAccepted).subscribe({
                next: (res: any) => {
                    showGlobalDialog('Upload file', res.message, true);
                    hideGlobalLoading();
                },
                error: (err) => {
                    if (err.error.data.isAccepted) {
                        this.isAccepted.set(err.error.data.isAccepted);
                    }
                    showGlobalDialog('Upload file', err.error.message, false);
                    hideGlobalLoading();
                },
            });
        }
    }
}
