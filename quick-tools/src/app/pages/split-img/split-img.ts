import { Component, signal } from '@angular/core';
import { Icon } from '../../shared/components/icon/icon';
import { DialogService } from '../../core/services/dialog-service';
import { FileSystemHelper } from '../../shared/helpers/file-system.helper';
import { SplitImageHelper } from '../../shared/helpers/split-image.helper';
import { FileHelper } from '../../shared/helpers/file.helper';
import { ImagePiece } from '../../core/models/image-piece.model';
import { FormsModule } from '@angular/forms';

@Component({
    selector: 'app-split-img',
    imports: [Icon, FormsModule],
    templateUrl: './split-img.html',
    styleUrl: './split-img.scss',
})
export class SplitImg {
    files: { id: string; file: File }[] = [];
    previewList = signal<UploadImage[]>([]);
    resultImages = signal<ImagePiece[]>([]);

    showPopup = false;
    currentPreview: string | null = null;
    rows: number = 1;
    columns: number = 1;

    constructor(private dialogService: DialogService) {}

    onFilesSelected(event: Event) {
        const input = event.target as HTMLInputElement;
        if (!input.files) return;
        const selectedFiles = Array.from(input.files) as File[];
        selectedFiles.forEach((file) => {
            const reader = new FileReader();
            const id = crypto.randomUUID();
            reader.onload = (e) => {
                this.previewList.update((list) => [
                    ...list,
                    {
                        id: id,
                        file_size: file.size,
                        base64: e.target!.result as string,
                        filename: file.name,
                        downloaded: false,
                    },
                ]);
            };
            reader.readAsDataURL(file);

            this.files.push({ id: id, file: file });
        });
    }

    removeFile(id: string) {
        this.previewList.update((list) => list.filter((item) => item.id !== id));
        this.files = this.files.filter((item) => item.id !== id);
    }

    openPreviewUpload(index: number) {
        this.currentPreview = this.previewList()[index].base64;
        this.showPopup = true;
    }

    openPreviewResult(index: number) {
        this.currentPreview = this.resultImages()[index].dataURL;
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
        if (this.files.length === 0) {
            this.dialogService.showToastMessage(
                true,
                'No image choosen',
                'Please choose at least 1 image',
                false
            );
            return;
        }

        if (
            !Number.isInteger(this.rows) ||
            this.rows <= 0 ||
            !Number.isInteger(this.columns) ||
            this.columns <= 0
        ) {
            this.dialogService.showToastMessage(
                true,
                'Invalid rows and columns',
                'Please input positive integer',
                false
            );
            return;
        }

        this.dialogService.showLoadingDialog(true);
        const file = this.files[0].file;
        let base64 = await FileHelper.fileToBase64(file);
        const r = await SplitImageHelper.splitImage(base64, this.rows, this.columns);
        this.resultImages.set(r);
        this.dialogService.showLoadingDialog(false);
    }

    async downloadBase64Image(img: ImagePiece) {
        img.downloaded = true;
        await FileSystemHelper.writeImgToPicturesFromBase64(crypto.randomUUID(), img.dataURL);
    }
}
