import { Component, signal } from '@angular/core';
import { Icon } from '../../shared/components/icon/icon';
import { FileSystemHelper } from '../../shared/helpers/file-system.helper';
import { TauriCommandService } from '../../core/services/tauri-command-service';
import { DialogService } from '../../core/services/dialog-service';
import { Commands } from '../../core/enums/commands.enum';
import {
    BinaryFile,
    ILoveImgUpscalePayloadCommand,
} from '../../core/models/payload-commands/IloveImg-upscale.payload-command';

@Component({
    selector: 'app-iloveimg-upscale-image',
    imports: [Icon],
    templateUrl: './iloveimg-upscale-image.html',
    styleUrl: './iloveimg-upscale-image.scss',
})
export class IloveimgUpscaleImage {
    MAX_FILES = 5;
    MAX_SIZE = 10 * 1024 * 1024; // 10MB
    MAX_TOTAL_SIZE = 15 * 1024 * 1024; // 15MB

    files: { id: string; file: File }[] = [];
    previewList = signal<UpscaleResult[]>([]);
    invalidImage = signal<UpscaleResult[]>([]);
    resultImages = signal<UpscaleResult[]>([]); // image list result from api

    // popup details
    showPopup = false;
    currentPreview: string | null = null;

    constructor(
        private tauriCommandService: TauriCommandService,
        private dialogService: DialogService,
    ) {}

    onFilesSelected(event: Event) {
        const input = event.target as HTMLInputElement;
        if (!input.files) return;

        const MAX_PIXELS = 33177600;
        const selectedFiles = Array.from(input.files) as File[];

        selectedFiles.forEach((file) => {
            if (!file.type.startsWith('image/')) return;

            const img = new Image();
            const objectUrl = URL.createObjectURL(file);

            img.onload = () => {
                const width = img.width;
                const height = img.height;
                const totalPixels = width * height;

                URL.revokeObjectURL(objectUrl);

                // ❌ Không đạt điều kiện pixel
                if (totalPixels >= MAX_PIXELS) {
                    console.warn(
                        `Ảnh ${file.name} bị loại: ${width}x${height} = ${totalPixels} pixels`,
                    );
                    const canvas = document.createElement('canvas');
                    canvas.width = img.width;
                    canvas.height = img.height;
                    const ctx = canvas.getContext('2d')!;
                    ctx.drawImage(img, 0, 0);
                    const base64 = canvas.toDataURL(file.type);

                    this.invalidImage.update((list) => [
                        ...list,
                        {
                            id,
                            file_size: file.size,
                            base64: base64,
                            filename: file.name,
                            downloaded: false,
                        },
                    ]);
                    return;
                }

                const id = crypto.randomUUID();
                this.files.push({ id, file });

                const reader = new FileReader();
                reader.onload = (e) => {
                    this.previewList.update((list) => [
                        ...list,
                        {
                            id,
                            file_size: file.size,
                            base64: e.target!.result as string,
                            filename: file.name,
                            downloaded: false,
                        },
                    ]);
                };

                reader.readAsDataURL(file);
            };

            img.onerror = () => {
                URL.revokeObjectURL(objectUrl);
                console.error(`Không đọc được ảnh: ${file.name}`);
            };

            img.src = objectUrl;
        });

        input.value = '';
    }

    removeFile(id: string) {
        this.previewList.update((list) => list.filter((item) => item.id !== id));
        this.files = this.files.filter((item) => item.id !== id);
    }

    openPreview(index: number) {
        this.currentPreview = this.previewList()[index].base64;
        this.showPopup = true;
    }

    openPreviewInvalid(index: number) {
        this.currentPreview = this.invalidImage()[index].base64;
        this.showPopup = true;
    }

    openPreviewResult(index: number) {
        this.currentPreview = this.resultImages()[index].base64;
        this.showPopup = true;
    }

    closePopup() {
        this.showPopup = false;
    }

    formatSize(size: number) {
        if (size < 1024) return size + ' B';
        if (size < 1024 * 1024) return (size / 1024).toFixed(1) + ' KB';
        return (size / (1024 * 1024)).toFixed(1) + ' MB';
    }

    async submit() {
        const isValid = this.validateFiles();

        if (!isValid) {
            return;
        }

        this.dialogService.showLoadingDialog(true);
        const binaryFiles: BinaryFile[] = [];
        for (const file of this.files) {
            const buffer = await file.file.arrayBuffer();
            const bytes = Array.from(new Uint8Array(buffer));

            binaryFiles.push({
                name: file.file.name,
                bytes: bytes,
            });
        }

        const payload: ILoveImgUpscalePayloadCommand = {
            files: binaryFiles,
        };
        const r = await this.tauriCommandService.invokeCommand<UpscaleResult[]>(
            Commands.ILOVEIMG_UPSCALE_IMG_COMMAND,
            payload,
            true,
        );
        if (r === null) {
            return;
        }

        const previews: UpscaleResult[] = r.map((r) => ({
            id: crypto.randomUUID(),
            filename: r.filename,
            base64: 'data:image/jpeg;base64,' + r.base64, // ✅ thêm prefix
            file_size: r.file_size,
            downloaded: false,
        }));

        this.resultImages.set(previews);
        for (const i of this.resultImages()) {
            console.log(i);
        }
    }

    async downloadBase64Image(img: UpscaleResult) {
        img.downloaded = true;
        await FileSystemHelper.writeImgToPicturesFromBase64(img.filename, img.base64);
    }

    validateFiles() {
        if (this.files.length === 0) {
            this.dialogService.showToastMessage(
                true,
                'No image choosen',
                'Please choose at least 1 image',
                false,
            );
            return false;
        }

        if (this.files.length > this.MAX_FILES) {
            this.dialogService.showToastMessage(
                true,
                `Max files is ${this.MAX_FILES} files`,
                `You can only select a maximum of ${this.MAX_FILES} files.`,
                false,
            );
            return false;
        }

        let totalSize = 0;
        for (const item of this.files) {
            totalSize += item.file.size;
            if (item.file.size > this.MAX_SIZE) {
                this.dialogService.showToastMessage(
                    true,
                    `Max size per file is ${this.MAX_SIZE} mb`,
                    `The file "${item.file.name}" exceeds 10MB.`,
                    false,
                );
                return false;
            }
        }

        if (totalSize > this.MAX_TOTAL_SIZE) {
            this.dialogService.showToastMessage(
                true,
                `Exceeded total capacity ${this.formatSize(totalSize)}`,
                `The total size exceeds 15MB (currently ${this.formatSize(totalSize)})`,
                false,
            );
            return false;
        }
        return true;
    }
}
