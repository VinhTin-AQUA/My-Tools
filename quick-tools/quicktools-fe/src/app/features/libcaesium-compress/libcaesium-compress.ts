import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MessageService } from '@openng/optimus-ui/api';
import { BadgeModule } from '@openng/optimus-ui/badge';
import { ButtonModule } from '@openng/optimus-ui/button';
import { FileUploadModule } from '@openng/optimus-ui/fileupload';
import { ProgressBarModule } from '@openng/optimus-ui/progressbar';
import { ToastModule } from '@openng/optimus-ui/toast';
import { InputNumberModule } from '@openng/optimus-ui/inputnumber';
import { FileUploadComponent } from '../../components/file-upload.component/file-upload.component';
import { NavigationComponent } from '../../components/navigation.component/navigation.component';
import { FileSizePipe } from '../../pipes/file-size-pipe';
import { ProcessingStatus, FileModel } from '../../models/file.model';
import { WebuiService } from '../../services/webui-service';
import { Notification } from '../../models/notification';
import {
    LibcaesiumCompressImageRequest,
    LibcaesiumCompressImageRequestItem,
    LibcaesiumCompressImageResponseItem,
} from './models/libcaesium-compress';

@Component({
    selector: 'app-libcaesium-compress',
    imports: [
        FormsModule,
        ButtonModule,
        FileUploadModule,
        BadgeModule,
        FileUploadModule,
        ProgressBarModule,
        FileUploadComponent,
        FileSizePipe,
        ToastModule,
        NavigationComponent,
        InputNumberModule,
    ],
    templateUrl: './libcaesium-compress.html',
    styleUrl: './libcaesium-compress.css',
    providers: [MessageService],
})
export class LibcaesiumCompress {
    protected readonly FileProcessingStatus = ProcessingStatus;
    quality: number = 30;
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
        (window as any).compressNotification = (data: ArrayBuffer) => {
            this.compressNotification(data);
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
        const updatedFiles = await Promise.all(
            files.map(async (updatedFile) => {
                const item: LibcaesiumCompressImageRequestItem = {
                    id: updatedFile.id,
                    name: updatedFile.name,
                    base64: updatedFile.base64,
                    localPath: '',
                };

                return item;
            }),
        );

        const data: LibcaesiumCompressImageRequest = {
            quality: this.quality,
            compressImageRequestItems: updatedFiles,
        };

        console.log(data);
        
        await this.webuiService.callJson('libcaesiumCompressImage', data);
    }

    onClear(): void {
        this.uploadedFiles.set([]);
        this.processedImages.set([]);
        this.isUploading.set(false);
    }

    toggleSelect(image: FileModel): void {
        image.selected = !image.selected;
    }

    compressNotification(data: ArrayBuffer) {
        const decoder = new TextDecoder('utf-8');
        const jsonString = decoder.decode(data);
        const notification = JSON.parse(
            jsonString,
        ) as Notification<LibcaesiumCompressImageResponseItem>;
        const compressImageResponseItem = notification.data;

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
                file.id === compressImageResponseItem.id
                    ? { ...file, processingStatus: compressImageResponseItem.processingStatus }
                    : file,
            ),
        );

        this.processedImages.update((images) => [
            ...images,
            {
                base64: '',
                name: compressImageResponseItem.name,
                size: compressImageResponseItem.size,
                id: compressImageResponseItem.id,
                previewUrl: compressImageResponseItem.localPath,
                processingStatus: compressImageResponseItem.processingStatus,
            },
        ]);

        if (this.totalSizeProcessed() === this.processedImages().length) {
            this.isUploading.set(false);
        }
    }

    async openFolder(folderPath: string) {
        await this.webuiService.call('openFolder', folderPath);
    }

    ngOnDestroy() {
        for (let prev of this.uploadedFiles()) {
            URL.revokeObjectURL(prev.previewUrl);
        }
        this.onClear();

        if ((window as any).compressNotification) {
            delete (window as any).compressNotification;
            console.log('🧹 Compress Notification removed from window');
        }
    }
}
