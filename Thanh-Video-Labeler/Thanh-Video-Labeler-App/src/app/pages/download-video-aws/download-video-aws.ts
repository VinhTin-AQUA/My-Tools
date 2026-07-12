import {
    Component,
    ElementRef,
    inject,
    signal,
    ViewChild,
} from '@angular/core';
import { VideoAwsService } from './services/video-aws-service';
import {
    hideGlobalLoading,
    showGlobalLoading,
} from '../../shared/signals/loading-signal';
import { showGlobalDialog } from '../../shared/signals/dialog-signal';
import { AwsVideoDownlingStore } from './signals/aws-video-downloading-signal';
import { VideoAwsHub } from './hubs/video-aws-hub';
import { AwsVideoDownlingErrorStore } from './signals/aws-video-downloading-error-signal';
import { AwsVideoTotalSuccessStore } from './signals/aws-video-total-success-signal';

@Component({
    selector: 'app-download-video-aws',
    imports: [],
    templateUrl: './download-video-aws.html',
    styleUrl: './download-video-aws.scss',
})
export class DownloadVideoAws {
    @ViewChild('fileInput') fileInput!: ElementRef;
    selectedFile: File | null = null;
    fileName = signal<string>('no file');
    totalVideo = signal<number>(0);

    showWarningUploadVideo = signal<boolean>(false);
    awsVideoDownlingStore = inject(AwsVideoDownlingStore);
    awsVideoDownlingErrorStore = inject(AwsVideoDownlingErrorStore);
    isDownloading = signal<boolean>(false);
    awsVideoTotalSuccessStore = inject(AwsVideoTotalSuccessStore);

    constructor(
        private videoAwsService: VideoAwsService,
        private videoAwsHub: VideoAwsHub
    ) {}

    ngOnInit() {
        this.videoAwsHub.startConnection();
        this.getRemaning();
    }

    onFileSelected(event: any): void {
        const file = event.target.files?.[0];
        if (file) {
            this.selectedFile = file;
        }
    }

    toggleOpenWarningUpload(flag: boolean) {
        if (this.selectedFile === null) {
            showGlobalDialog('Upload File', 'Choose excel file', false);
            return;
        }

        this.showWarningUploadVideo.set(flag);
    }

    upload() {
        if (this.selectedFile === null) {
            showGlobalDialog('Upload File', 'Choose excel file', false);
            return;
        }
        showGlobalLoading();
        this.toggleOpenWarningUpload(false);

        let formData = new FormData();
        formData.append('File', this.selectedFile);

        this.videoAwsService.uploadVideo(formData).subscribe({
            next: (res: any) => {
                this.fileName.set(res.data.fileName);
                this.awsVideoTotalSuccessStore.increase(
                    res.data.totalDownloaded
                );
                this.totalVideo.set(res.data.totalVideo);

                hideGlobalLoading();
                this.fileInput.nativeElement.value = '';
            },
            error: (err) => {
                showGlobalDialog('Upload File', err.error.message, false);
                hideGlobalLoading();
            },
        });
    }

    getRemaning() {
        showGlobalLoading();

        this.videoAwsService.getRemaining().subscribe({
            next: (res: any) => {
                this.fileName.set(res.data.fileName);

                this.awsVideoTotalSuccessStore.increase(
                    res.data.totalDownloaded
                );
                this.totalVideo.set(res.data.totalVideo);
                hideGlobalLoading();
            },
            error: (err) => {
                showGlobalDialog('Upload File', err.error.message, false);
                hideGlobalLoading();
            },
        });
    }

    startDownload() {
        this.isDownloading.set(true);

        this.videoAwsService.startDownload().subscribe({
            next: (res) => {},
            error: (err) => {
                showGlobalDialog('Upload File', err.error.message, false);
            },
        });
    }

    stopDownload() {
        this.isDownloading.set(false);

        this.videoAwsService.stopDownload().subscribe({
            next: (res) => {},
            error: (err) => {
                showGlobalDialog('Upload File', err.error.message, false);
            },
        });
    }

    copyToClipboard(text: string) {
        navigator.clipboard
            .writeText(text)
            .then(() => {})
            .catch((err) => {});
    }
}
