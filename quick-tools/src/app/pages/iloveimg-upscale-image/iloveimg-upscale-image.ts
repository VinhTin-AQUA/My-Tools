import { Component, signal } from '@angular/core';
import { getCurrentWebview } from '@tauri-apps/api/webview';
import { Icon } from '../../shared/components/icon/icon';
import { TauriCommandService } from '../../core/services/tauri-command-service';
import { DialogService } from '../../core/services/dialog-service';
import { Commands } from '../../core/constants/commands.enum';
import { SelectBox } from '../../shared/components/select-box/select-box';
import { OptionModel } from '../../core/models/option.model';
import { Button } from '../../shared/components/button/button';
import { listen, UnlistenFn } from '@tauri-apps/api/event';
import { EmitEvents } from '../../core/constants/emit_events';
import {
    UpscaleImageRequest,
    UpscaleImageResult,
    UpscaleReviewResult,
} from '../../core/models/upload-image.model';
import { ILoveImgUpscalePayloadCommand } from '../../core/models/payload-commands/IloveImg-upscale.payload-command';
import { IMAGE_EXTENSIONS } from '../../core/constants/file-extensions';
import { NotificationHelper } from '../../shared/helpers/notification.helper';
import { FileHelper, SelectedFile } from '../../shared/helpers/file.helper';
import { convertFileSrc } from '@tauri-apps/api/core';
import { AppConstants } from '../../core/constants/app_constants';

@Component({
    selector: 'app-iloveimg-upscale-image',
    imports: [Icon, SelectBox, Button],
    templateUrl: './iloveimg-upscale-image.html',
    styleUrl: './iloveimg-upscale-image.scss',
})
export class IloveimgUpscaleImage {
    selectedFiles = signal<SelectedFile[]>([]);
    upscaleReviewResults = signal<UpscaleReviewResult[]>([]);
    scale: string = '1';
    scaleOptions: OptionModel[] = [
        { label: '1', value: '1' },
        { label: '2', value: '2' },
    ];

    showPopup = false;
    currentPreview: string | null = null;
    upscaleUnlistenEvent: UnlistenFn | null = null;
    dragAndDropUnlistenEvent: UnlistenFn | null = null;
    submitted = signal<boolean>(false);
    processedCount = signal<number>(0);

    constructor(
        private tauriCommandService: TauriCommandService,
        private dialogService: DialogService,
    ) {}

    async ngOnInit() {
        // await NotificationHelper.sendNotification('Saved', 'event.payload.path');
        // this.upscaleUnlistenEvent = await listen<UpscaleImageResult>(
        //     EmitEvents.UP_SCALE_IMAGE_RESULT,
        //     async (event) => {
        //         const src = await FileHelper.fileToBlobUrl(event.payload.path);
        //         this.resultImages.update((x) => {
        //             const newX: UpscaleResult = {
        //                 id: event.payload.id,
        //                 filename: event.payload.path.split('/').pop() ?? '',
        //                 file_size: 0,
        //                 src: src,
        //                 downloaded: false,
        //                 phisicalPath: event.payload.path,
        //             };
        //             return [...x, newX];
        //         });
        //         this.processedCount.update((x) => x + 1);
        //         this.previewList.update((images) =>
        //             images.map((img) =>
        //                 img.id === event.payload.id ? { ...img, downloaded: true } : img,
        //             ),
        //         );
        //         await NotificationHelper.sendNotification('Saved', event.payload.path);
        //     },
        // );
        // this.dragAndDropUnlistenEvent = await getCurrentWebview().onDragDropEvent(async (event) => {
        //     if (event.payload.type === 'over') {
        //         // console.log('User hovering', event.payload.position);
        //     } else if (event.payload.type === 'drop') {
        //         console.log('User dropped', event.payload.paths);
        //         const imagePaths = event.payload.paths.filter((path) =>
        //             IMAGE_EXTENSIONS.some((ext) => path.toLowerCase().endsWith(`.${ext}`)),
        //         );
        //         const list = await Promise.all(
        //             imagePaths.map(async (p, index) => {
        //                 const src = await await FileHelper.fileToBlobUrl(p);
        //                 const t: UpscaleResult = {
        //                     id: crypto.randomUUID(),
        //                     filename: p.split('/').pop() ?? '',
        //                     file_size: 0,
        //                     src: src,
        //                     downloaded: false,
        //                     phisicalPath: p,
        //                 };
        //                 return t;
        //             }),
        //         );
        //         this.previewList.update((current) => {
        //             const existingPaths = new Set(current.map((x) => x.phisicalPath));
        //             const filtered = list.filter((item) => !existingPaths.has(item.phisicalPath));
        //             return [...current, ...filtered];
        //         });
        //     } else {
        //     }
        // });
    }

    async onFilesSelected() {
        const files = await FileHelper.selectMultiFiles();

        if (!files) {
            return;
        }
        this.selectedFiles.set(files);
    }

    removeFile(id: string) {
        this.selectedFiles.update((list) => list.filter((item) => item.id !== id));
    }

    openPreview(index: number) {
        this.currentPreview = this.selectedFiles()[index].previewUrl;
        this.showPopup = true;
    }

    openPreviewResult(index: number) {
        this.currentPreview = this.upscaleReviewResults()[index].previewSrc;
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
        if (this.submitted() == true) {
            this.dialogService.showToastMessage(
                true,
                'Process is running',
                'Please wait for current process',
                false,
            );
            return;
        }
        this.selectedFiles.update((images) => images.map((img) => ({ ...img, downloaded: false })));

        this.submitted.set(true);
        const check = this.validateFiles();
        if (!check) {
            return;
        }
        this.upscaleReviewResults.set([]);
        this.processedCount.set(0);

        const payload: ILoveImgUpscalePayloadCommand = {
            files: this.selectedFiles(),
            scale: this.scale,
        };

        const r = await this.tauriCommandService.invokeCommand<UpscaleImageResult[]>(
            Commands.ILOVEIMG_UPSCALE_IMG_COMMAND,
            payload,
        );

        this.submitted.set(false);
        if (!r) {
            this.dialogService.showToastMessage(
                true,
                'Error upscale images',
                'Please try again',
                false,
            );
            return;
        }

        const upscaleReviewResultData: UpscaleReviewResult[] = await Promise.all(
            r.map(async (x) => {
                const info = await FileHelper.getStatOfFile(x.path);

                const temp: UpscaleReviewResult = {
                    file_size: info.size ?? 0,
                    fileName: x.fileName,
                    id: x.id,
                    previewSrc: convertFileSrc(x.path),
                };

                return temp;
            }),
        );
        this.upscaleReviewResults.set(upscaleReviewResultData);
    }

    validateFiles() {
        const MAX_FILES = 20;
        const MAX_SIZE = 10 * 1024 * 1024; // 10MB
        const MAX_TOTAL_SIZE = 15 * 1024 * 1024; // 15MB

        if (this.selectedFiles().length === 0) {
            this.dialogService.showToastMessage(
                true,
                'No image choosen',
                'Please choose at least 1 image',
                false,
            );
            return false;
        }

        if (this.selectedFiles().length > MAX_FILES) {
            this.dialogService.showToastMessage(
                true,
                `Max files is ${MAX_FILES} files`,
                `You can only select a maximum of ${MAX_FILES} files.`,
                false,
            );
            return false;
        }

        let totalSize = 0;
        for (const item of this.selectedFiles()) {
            totalSize += item.size;
            if (item.size > MAX_SIZE) {
                this.dialogService.showToastMessage(
                    true,
                    `Max size per file is ${MAX_SIZE} mb`,
                    `The file "${item.fileName}" exceeds 10MB.`,
                    false,
                );
                return false;
            }
        }

        if (totalSize > MAX_TOTAL_SIZE) {
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

    ngDestroy() {
        this.upscaleUnlistenEvent?.();
        this.dragAndDropUnlistenEvent?.();
    }
}
