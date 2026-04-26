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
import { SelectBox } from '../../shared/components/select-box/select-box';
import { OptionModel } from '../../core/models/option.model';
import { DialogHelper } from '../../shared/helpers/dialog.helper';
import { Button } from '../../shared/components/button/button';
import { convertFileSrc } from '@tauri-apps/api/core';

@Component({
    selector: 'app-iloveimg-upscale-image',
    imports: [Icon, SelectBox, Button],
    templateUrl: './iloveimg-upscale-image.html',
    styleUrl: './iloveimg-upscale-image.scss',
})
export class IloveimgUpscaleImage {
    MAX_FILES = 5;
    MAX_SIZE = 10 * 1024 * 1024; // 10MB
    MAX_TOTAL_SIZE = 15 * 1024 * 1024; // 15MB

    previewList = signal<UpscaleResult[]>([]);
    // invalidImage = signal<UpscaleResult[]>([]);
    resultImages = signal<UpscaleResult[]>([]); // image list result from api
    scale: string = '1';
    scaleOptions: OptionModel[] = [
        { label: '1', value: '1' },
        { label: '2', value: '2' },
    ];

    // popup details
    showPopup = false;
    currentPreview: string | null = null;

    constructor(
        private tauriCommandService: TauriCommandService,
        private dialogService: DialogService,
    ) {}

    ngOnInit() {
        // const r = [
        //     '/home/newtun/Downloads/upscale/upscaled_IMG_20260419_121538.jpg',
        //     '/home/newtun/Downloads/1775370634348.jpg',
        //     // '/home/newtun/Downloads/upscale/upscaled_IMG_20260419_121810.jpg',
        //     // '/home/newtun/Downloads/upscale/upscaled_IMG_20260419_122008.jpg',
        //     // '/home/newtun/Downloads/upscale/upscaled_IMG_20260419_122020.jpg',
        // ];

        // const list = r.map((p, index) => {
        //     const t: UpscaleResult = {
        //         id: crypto.randomUUID(),
        //         filename: p.split('/').pop() ?? '',
        //         file_size: 0,
        //         src: convertFileSrc(p),
        //         downloaded: false,
        //         phisicalPath: p,
        //     };
        //     return t;
        // });
        // this.resultImages.set(list);
    }

    async onFilesSelected() {
        const files = await DialogHelper.selectMultiFiles();
        if (!files) {
            this.dialogService.showToastMessage(
                true,
                'Error image selected',
                'Please try again',
                false,
            );
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
        const check = this.validateFiles();
        if (!check) {
            return;
        }

        const files = this.previewList().map((x) => x.phisicalPath);
        const payload: ILoveImgUpscalePayloadCommand = {
            files: files,
            scale: this.scale,
        };

        const r = await this.tauriCommandService.invokeCommand<string[]>(
            Commands.ILOVEIMG_UPSCALE_IMG_COMMAND,
            payload,
            true,
        );

        if (!r) {
            this.dialogService.showToastMessage(
                true,
                'Error upscale images',
                'Please try again',
                false,
            );
            return;
        }

        console.log(r);

        const list = r.map((p, index) => {
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
        this.resultImages.set(list);
    }

    validateFiles() {
        if (this.previewList().length === 0) {
            this.dialogService.showToastMessage(
                true,
                'No image choosen',
                'Please choose at least 1 image',
                false,
            );
            return false;
        }

        if (this.previewList().length > this.MAX_FILES) {
            this.dialogService.showToastMessage(
                true,
                `Max files is ${this.MAX_FILES} files`,
                `You can only select a maximum of ${this.MAX_FILES} files.`,
                false,
            );
            return false;
        }

        let totalSize = 0;
        for (const item of this.previewList()) {
            totalSize += item.file_size;
            if (item.file_size > this.MAX_SIZE) {
                this.dialogService.showToastMessage(
                    true,
                    `Max size per file is ${this.MAX_SIZE} mb`,
                    `The file "${item.filename}" exceeds 10MB.`,
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
