import { Component, computed, inject, signal } from '@angular/core';
import { MessageService } from '@openng/optimus-ui/api';
import { ProcessingStatus, FileModel } from '../../models/file.model';
import { WebuiService } from '../../services/webui-service';
import {
    UpscaleImageRequestItem,
    UpscaleImageRequest,
    UpscaleImageResponseItem,
} from '../upscale-image/models/upscale_image';
import { Notification } from '../../models/notification';

import { FormsModule } from '@angular/forms';
import { SelectModule } from '@openng/optimus-ui/select';
import { ButtonModule } from '@openng/optimus-ui/button';
import { FileUploadModule } from '@openng/optimus-ui/fileupload';
import { BadgeModule } from '@openng/optimus-ui/badge';
import { ProgressBarModule } from '@openng/optimus-ui/progressbar';
import { FileUploadComponent } from '../../components/file-upload.component/file-upload.component';
import { FileSizePipe } from '../../pipes/file-size-pipe';
import { ToastModule } from '@openng/optimus-ui/toast';
import { NavigationComponent } from '../../components/navigation.component/navigation.component';

@Component({
    selector: 'app-canvas-compress-image',
    imports: [
        FormsModule,
        SelectModule,
        ButtonModule,
        FileUploadModule,
        BadgeModule,
        ButtonModule,
        FileUploadModule,
        ProgressBarModule,
        FileUploadComponent,
        FileSizePipe,
        ToastModule,
        NavigationComponent,
    ],
    templateUrl: './canvas-compress-image.html',
    styleUrl: './canvas-compress-image.css',
    providers: [MessageService],
})
export class CanvasCompressImage {
    protected readonly FileProcessingStatus = ProcessingStatus;
    compressQuality: number = 30;
    compressQualityOptions = [10, 20, 30, 40, 50, 60, 70, 80, 90, 100];
    isUploading = signal<boolean>(false);

    uploadedFiles = signal<FileModel[]>([]);
    totalSize = computed(() => {
        return this.uploadedFiles().reduce((sum, file) => sum + file.size, 0);
    });

    selectedImage: FileModel | null = null;
    showPopupImagePreview: boolean = false;

    processedImages = signal<FileModel[]>([]);
    progress = computed(() => {
        return this.processedImages().length;
    });
    totalSizeProcessed = computed(() => {
        return this.processedImages().reduce((sum, file) => sum + file.size, 0);
    });

    private messageService = inject(MessageService);

    constructor(private webuiService: WebuiService) {}

    ngOnInit() {
        (window as any).scaleNotification = (data: ArrayBuffer) => {
            // this.scaleNotification(data);
        };
    }

    onImageClick(image: FileModel): void {
        this.selectedImage = image;
        this.showPopupImagePreview = true;
    }

    closePopupImagePreview(): void {
        this.showPopupImagePreview = false;
        this.selectedImage = null;
    }

    removeImage(event: MouseEvent, id: string) {
        event.preventDefault();
        event.stopPropagation();
        this.uploadedFiles.update((currentFiles) => currentFiles.filter((f) => f.id !== id));
    }

    async onSubmit() {
        if (this.isUploading()) {
            return;
        }

        if (this.uploadedFiles().length === 0) {
            return;
        }

        this.messageService.add({
            severity: 'success',
            summary: 'Scaling Started',
            detail: '',
        });

        this.isUploading.set(true);

        const files = this.uploadedFiles();

        if (!files.length) {
            this.processedImages.set([]);
            return;
        }

        const quality = this.compressQuality / 100;

        const results = await Promise.all(files.map((file) => this.compressImage(file, quality)));

        this.processedImages.set(results);

        // const updatedFiles = await Promise.all(
        //     files.map(async (updatedFile) => {
        //         const item: UpscaleImageRequestItem = {
        //             id: updatedFile.id,
        //             name: updatedFile.name,
        //             base64: updatedFile.base64,
        //             localPath: '',
        //         };

        //         return item;
        //     }),
        // );

        // const data: UpscaleImageRequest = {
        //     scale: this.compressedScale,
        //     upscaleImageRequestItems: updatedFiles,
        // };
        // await this.webuiService.callJson('upscaleImage', data);

        this.isUploading.set(false);
    }

    onClear(): void {
        this.uploadedFiles.set([]);
        this.processedImages.set([]);
        this.isUploading.set(false);
    }

    toggleSelect(image: FileModel): void {
        image.selected = !image.selected;
    }

    downloadImage(file: FileModel): void {
        const link = document.createElement('a');
        link.href = file.base64 || file.previewUrl;
        link.download = file.name;
        link.click();
    }

    private compressImage(file: FileModel, quality: number): Promise<FileModel> {
        return new Promise((resolve) => {
            const image = new Image();

            image.onload = () => {
                const canvas = document.createElement('canvas');

                canvas.width = image.naturalWidth;
                canvas.height = image.naturalHeight;

                // const context = canvas.getContext('2d');

                // if (!context) {
                //     resolve({
                //         ...file,
                //         processingStatus: ProcessingStatus.FAILED,
                //     });

                //     return;
                // }

                /**
                 * Giữ behavior của hàm cũ:
                 * fill background trắng trước khi draw JPEG.
                 */
                // context.fillStyle = '#FFFFFF';
                // context.fillRect(0, 0, canvas.width, canvas.height);

                // context.drawImage(image, 0, 0, canvas.width, canvas.height);

                const compressedBase64 = canvas.toDataURL('image/jpeg', quality);

                const compressedSize = this.base64Size(compressedBase64);

                const compressedName = this.getCompressedName(file.name);

                resolve({
                    ...file,

                    id: crypto.randomUUID(),
                    name: compressedName,
                    size: compressedSize,
                    base64: compressedBase64,
                    previewUrl: compressedBase64,
                    processingStatus: ProcessingStatus.SUCCESS,
                });
            };

            image.onerror = () => {
                resolve({
                    ...file,
                    processingStatus: ProcessingStatus.FAILED,
                });
            };

            /**
             * previewUrl thường là blob URL hoặc data URL.
             * Nếu component upload của bạn đã tạo previewUrl thì ưu tiên dùng nó.
             */
            image.src = file.previewUrl || file.base64;
        });
    }

    private base64Size(dataUrl: string): number {
        const base64 = dataUrl.split(',')[1] ?? '';

        /**
         * Kích thước thực tế gần đúng của binary data.
         */
        return Math.floor((base64.length * 3) / 4);
    }

    private getCompressedName(name: string): string {
        const lastDot = name.lastIndexOf('.');

        if (lastDot === -1) {
            return `${name}-compressed.jpg`;
        }

        return `${name.substring(0, lastDot)}-compressed.jpg`;
    }

    ngOnDestroy() {
        for (let prev of this.uploadedFiles()) {
            URL.revokeObjectURL(prev.previewUrl);
        }
        this.onClear();

        if ((window as any).scaleNotification) {
            // delete (window as any).scaleNotification;
            console.log('🧹 scaleNotification removed from window');
        }
    }
}
