import { Component, signal } from '@angular/core';
import { getCurrentWebview } from '@tauri-apps/api/webview';
import { Icon } from '../../shared/components/icon/icon';
import { TauriCommandService } from '../../core/services/tauri-command-service';
import { DialogService } from '../../core/services/dialog-service';
import { Commands } from '../../core/enums/commands.enum';
import { SelectBox } from '../../shared/components/select-box/select-box';
import { OptionModel } from '../../core/models/option.model';
import { DialogHelper } from '../../shared/helpers/dialog.helper';
import { Button } from '../../shared/components/button/button';
import { convertFileSrc } from '@tauri-apps/api/core';
import { listen, UnlistenFn } from '@tauri-apps/api/event';
import { EmitEvents } from '../../core/constants/emit_events';
import { UpscaleImageRequest, UpscaleImageResult } from '../../core/models/upload-image.model';
import { ILoveImgUpscalePayloadCommand } from '../../core/models/payload-commands/IloveImg-upscale.payload-command';
import { IMAGE_EXTENSIONS } from '../../core/constants/file-extensions';

@Component({
    selector: 'app-iloveimg-upscale-image',
    imports: [Icon, SelectBox, Button],
    templateUrl: './iloveimg-upscale-image.html',
    styleUrl: './iloveimg-upscale-image.scss',
})
export class IloveimgUpscaleImage {
    previewList = signal<UpscaleResult[]>([]);
    resultImages = signal<UpscaleResult[]>([]);
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

    constructor(
        private tauriCommandService: TauriCommandService,
        private dialogService: DialogService,
    ) {}

    async ngOnInit() {
        this.upscaleUnlistenEvent = await listen<UpscaleImageResult>(
            EmitEvents.UP_SCALE_IMAGE_RESULT,
            (event) => {
                console.log(event);

                this.resultImages.update((x) => {
                    const newX: UpscaleResult = {
                        id: event.payload.id,
                        filename: event.payload.path.split('/').pop() ?? '',
                        file_size: 0,
                        src: convertFileSrc(event.payload.path),
                        downloaded: false,
                        phisicalPath: event.payload.path,
                    };

                    return [...x, newX];
                });

                this.previewList.update((images) =>
                    images.map((img) =>
                        img.id === event.payload.id ? { ...img, downloaded: true } : img,
                    ),
                );
                window.scrollTo({
                    top: document.body.scrollHeight,
                    behavior: 'smooth',
                });
            },
        );

        this.dragAndDropUnlistenEvent = await getCurrentWebview().onDragDropEvent((event) => {
            if (event.payload.type === 'over') {
                // console.log('User hovering', event.payload.position);
            } else if (event.payload.type === 'drop') {
                console.log('User dropped', event.payload.paths);

                const imagePaths = event.payload.paths.filter((path) =>
                    IMAGE_EXTENSIONS.some((ext) => path.toLowerCase().endsWith(ext)),
                );

                const list = imagePaths.map((p, index) => {
                    const t: UpscaleResult = {
                        id: crypto.randomUUID(),
                        filename: p.split('/').pop() ?? '',
                        file_size: 0,
                        src: convertFileSrc(p),
                        downloaded: false,
                        phisicalPath: p,
                    };
                    return t;
                });

                this.previewList.update((current) => {
                    const existingPaths = new Set(current.map((x) => x.phisicalPath));
                    const filtered = list.filter((item) => !existingPaths.has(item.phisicalPath));

                    return [...current, ...filtered];
                });
            } else {
            }
        });
    }

    async onFilesSelected() {
        const files = await DialogHelper.selectMultiFiles();
        if (!files) {
            return;
        }

        const list = files.map((p, index) => {
            const t: UpscaleResult = {
                id: crypto.randomUUID(),
                filename: p.split('/').pop() ?? '',
                file_size: 0,
                src: convertFileSrc(p),
                downloaded: false,
                phisicalPath: p,
            };
            return t;
        });
        this.previewList.set(list);
    }

    removeFile(id: string) {
        this.previewList.update((list) => list.filter((item) => item.id !== id));
    }

    openPreview(index: number) {
        this.currentPreview = this.previewList()[index].src;
        this.showPopup = true;
    }

    openPreviewResult(index: number) {
        this.currentPreview = this.resultImages()[index].src;
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
        this.previewList.update((images) => images.map((img) => ({ ...img, downloaded: false })));

        this.submitted.set(true);
        const check = this.validateFiles();
        if (!check) {
            return;
        }
        this.resultImages.set([]);

        const files = this.previewList().map((x) => {
            const file: UpscaleImageRequest = {
                id: x.id,
                path: x.phisicalPath,
            };
            return file;
        });
        const payload: ILoveImgUpscalePayloadCommand = {
            files: files,
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
    }

    validateFiles() {
        const MAX_FILES = 20;
        const MAX_SIZE = 10 * 1024 * 1024; // 10MB
        const MAX_TOTAL_SIZE = 15 * 1024 * 1024; // 15MB

        if (this.previewList().length === 0) {
            this.dialogService.showToastMessage(
                true,
                'No image choosen',
                'Please choose at least 1 image',
                false,
            );
            return false;
        }

        if (this.previewList().length > MAX_FILES) {
            this.dialogService.showToastMessage(
                true,
                `Max files is ${MAX_FILES} files`,
                `You can only select a maximum of ${MAX_FILES} files.`,
                false,
            );
            return false;
        }

        let totalSize = 0;
        for (const item of this.previewList()) {
            totalSize += item.file_size;
            if (item.file_size > MAX_SIZE) {
                this.dialogService.showToastMessage(
                    true,
                    `Max size per file is ${MAX_SIZE} mb`,
                    `The file "${item.filename}" exceeds 10MB.`,
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
