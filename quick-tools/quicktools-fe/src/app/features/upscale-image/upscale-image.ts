import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { FileModel, ProcessingStatus } from '../../models/file.model';
import { SelectModule } from '@openng/optimus-ui/select';
import { ButtonModule } from '@openng/optimus-ui/button';
import { FileUploadModule } from '@openng/optimus-ui/fileupload';
import { BadgeModule } from '@openng/optimus-ui/badge';
import { ProgressBarModule } from '@openng/optimus-ui/progressbar';
import { FileUploadComponent } from '../../components/file-upload.component/file-upload.component';
import { FileSizePipe } from '../../pipes/file-size-pipe';
import { WebuiService } from '../../services/webui-service';

import { ToastModule } from '@openng/optimus-ui/toast';
// import { RippleModule } from '@openng/optimus-ui/ripple';
import { MessageService } from '@openng/optimus-ui/api';

import {
    UpscaleImageRequest,
    UpscaleImageRequestItem,
    UpscaleImageResponseItem,
} from './models/upscale_image';
import { Notification } from '../../models/notification';
import { NavigationComponent } from '../../components/navigation.component/navigation.component';

@Component({
    selector: 'app-upscale-image',
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
    templateUrl: './upscale-image.html',
    styleUrl: './upscale-image.css',
    providers: [MessageService],
})
export class UpscaleImage {
    protected readonly FileProcessingStatus = ProcessingStatus;
    scale: string = '1';
    sizeMultiplierOptions = ['1', '2'];
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
            this.scaleNotification(data);
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

        this.isUploading.set(true);
        const files = this.uploadedFiles();
        const updatedFiles = await Promise.all(
            files.map(async (updatedFile) => {
                const item: UpscaleImageRequestItem = {
                    id: updatedFile.id,
                    name: updatedFile.name,
                    base64: updatedFile.base64,
                    localPath: '',
                };

                return item;
            }),
        );

        const data: UpscaleImageRequest = {
            scale: this.scale,
            upscaleImageRequestItems: updatedFiles,
        };
        this.webuiService.callJson('upscaleImage', data);
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

    scaleNotification(data: ArrayBuffer) {
        const decoder = new TextDecoder('utf-8');
        const jsonString = decoder.decode(data);
        const notification = JSON.parse(jsonString) as Notification<UpscaleImageResponseItem>;
        const upscaleImageResponseItem = notification.data;

        if (!notification.isSuccess) {
            this.messageService.add({
                severity: 'error',
                summary: notification.title,
                detail: notification.message,
            });
            this.isUploading.set(false);

            return;
        }

        this.uploadedFiles.update((files) =>
            files.map((file) =>
                file.id === upscaleImageResponseItem.id
                    ? { ...file, processingStatus: upscaleImageResponseItem.processingStatus }
                    : file,
            ),
        );

        this.processedImages.update((images) => [
            ...images,
            {
                base64: '',
                name: upscaleImageResponseItem.name,
                size: upscaleImageResponseItem.size,
                id: upscaleImageResponseItem.id,
                previewUrl: upscaleImageResponseItem.localPath,
                processingStatus: upscaleImageResponseItem.processingStatus,
            },
        ]);

        if (this.totalSizeProcessed() === this.processedImages().length) {
            this.isUploading.set(false);
        }
    }

    openFolder(folderPath: string) {
        this.webuiService.call('openFolder', folderPath);
    }

    ngOnDestroy() {
        for (let prev of this.uploadedFiles()) {
            URL.revokeObjectURL(prev.previewUrl);
        }
        this.onClear();

        if ((window as any).scaleNotification) {
            delete (window as any).scaleNotification;
            console.log('🧹 scaleNotification removed from window');
        }
    }
}
