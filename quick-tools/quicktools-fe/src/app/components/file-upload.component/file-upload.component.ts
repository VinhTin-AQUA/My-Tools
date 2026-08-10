import { Component, input, model, output } from '@angular/core';
import { ButtonModule } from '@openng/optimus-ui/button';
import { FileSelectEvent, FileUploadModule } from '@openng/optimus-ui/fileupload';
import { TagModule } from '@openng/optimus-ui/tag';
import { FileModel, ProcessingStatus } from '../../models/file.model';
import { fileToBase64 } from '../../helpers/files.helper';

@Component({
    selector: 'file-upload-component',
    imports: [ButtonModule, FileUploadModule, TagModule],
    templateUrl: './file-upload.component.html',
    styleUrl: './file-upload.component.css',
})
export class FileUploadComponent {
    files = model<FileModel[]>([]);
    isDragging = false;

    onSelectFile(event: FileSelectEvent) {
        const fileList = event.currentFiles || [];
        const filesArray = Array.isArray(fileList) ? fileList : [];
        this.handleSelectFiles(filesArray);

        // filesArray.forEach((file, index) => {
        //     console.log(`File ${index + 1}: ${file.name} (${file.size} bytes)`);
        // });
    }

    onDragOver(event: DragEvent) {
        event.preventDefault();
        event.stopPropagation();
        this.isDragging = true;
    }

    // Khi file rời khỏi vùng upload
    onDragLeave(event: DragEvent) {
        event.preventDefault();
        event.stopPropagation();

        this.isDragging = false;
    }

    // Khi thả file
    onDrop(event: DragEvent) {
        event.preventDefault();
        event.stopPropagation();

        this.isDragging = false;

        const fileList = event.dataTransfer?.files;

        if (!fileList || fileList.length === 0) {
            return;
        }

        const filesArray = Array.from(fileList).filter((file) => file.type.startsWith('image/'));

        this.handleSelectFiles(filesArray);
    }

    async handleSelectFiles(filesArray: File[]) {
        const files = await Promise.all(
            filesArray.map(async (f) => {
                const file: FileModel = {
                    id: crypto.randomUUID(),
                    name: f.name,
                    size: f.size,
                    previewUrl: URL.createObjectURL(f),
                    base64: await fileToBase64(f),
                    processingStatus: ProcessingStatus.PROCESSING,
                    selected: false,
                };

                return file;
            }),
        );

        this.files.update((currentFiles) => [...currentFiles, ...files]);
    }
}
