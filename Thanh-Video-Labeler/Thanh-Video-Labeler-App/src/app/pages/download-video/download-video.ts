import { CommonModule } from '@angular/common';
import { Component, computed, signal } from '@angular/core';
import { DownloadVideoService } from './services/download-video-service';
import { showGlobalDialog } from '../../shared/signals/dialog-signal';
import {
    hideGlobalLoading,
    showGlobalLoading,
} from '../../shared/signals/loading-signal';
import { VideoInfo } from './models/video-info';
import { VideoHub } from './hubs/video-hub';
import { resultDownloadVideoSignal } from './signal/total-result-signal';
import { FormsModule } from '@angular/forms';

@Component({
    selector: 'app-download-video',
    imports: [CommonModule, FormsModule],
    templateUrl: './download-video.html',
    styleUrl: './download-video.scss',
})
export class DownloadVideo {
    isDownloading = signal<boolean>(false);
    isLoading = signal<boolean>(false);

    resultDownloadVideoSignal = computed(() => resultDownloadVideoSignal());
    sampleVideos = signal<VideoInfo[]>([]);
    pendingTotalVideos = signal<number>(0);
    downloadedTotalVideos = signal<number>(0);
    errorLinkTotalVideos = signal<number>(0);
    seletectedSheet = signal<{ sheetName: string; sheetCode: string }>({
        sheetCode: '',
        sheetName: '',
    });
    sheets = signal<any>([]);
    totalToDownload: number = 500;

    // dropdown
    isOpenDropdown = false;

    constructor(
        private videoService: DownloadVideoService,
        private videoHub: VideoHub
    ) {}

    ngOnInit(): void {
        this.videoHub.startConnection();
        this.getRemainingVideos();

        if (this.videoHub.hubConnection) {
            this.videoHub.hubConnection.on('SendDownloadFinish', (isFinish) => {
                console.log(isFinish);
                this.updateLoading(false);
                showGlobalDialog('Dowload', 'Dowload Successfully', true);
            });
        }
    }

    getRemainingVideos() {
        showGlobalLoading();
        this.videoService.getRemainingVideos().subscribe({
            next: (res: any) => {
                hideGlobalLoading();
                this.sampleVideos.update((x) => res.data.sampleVideos);
                this.pendingTotalVideos.update(
                    (x) => res.data.pendingTotalVideos
                );
                this.downloadedTotalVideos.update(
                    (x) => res.data.downloadedTotalVideos
                );
                this.errorLinkTotalVideos.update(
                    (x) => res.data.errorLinkTotalVideos
                );
                this.seletectedSheet.update((x) => {
                    return {
                        sheetCode: res.data.selectedSheet.sheetCode,
                        sheetName: res.data.selectedSheet.sheetName,
                    };
                });
                this.sheets.update((x) => res.data.sheets);
            },
            error: (err) => {
                hideGlobalLoading();
            },
        });
    }

    initVideos() {
        showGlobalLoading();
        this.videoService
            .initVideo({
                sheetName: this.seletectedSheet().sheetName,
                sheetCode: this.seletectedSheet().sheetCode,
            })
            .subscribe({
                next: (res: any) => {
                    showGlobalDialog('Init download', res.message, true);
                    this.sampleVideos.update((x) => res.data.sampleVideos);
                    this.pendingTotalVideos.update(
                        (x) => res.data.pendingTotalVideos
                    );
                    this.seletectedSheet.update((x) => {
                        return {
                            sheetCode: res.data.selectedSheet.sheetCode,
                            sheetName: res.data.selectedSheet.sheetName,
                        };
                    });
                    hideGlobalLoading();
                },
                error: (err) => {
                    showGlobalDialog('Init download', err.error.message, false);
                    hideGlobalLoading();
                },
            });
    }

    // stop
    toggleDownload() {
        if (this.isDownloading()) {
            this.updateLoading(false);
            this.videoService.stopDownloadVideo().subscribe({
                next: (res: any) => {
                    showGlobalDialog('Stop Dowload', res.message, true);
                },
                error: (err) => {
                    showGlobalDialog('Stop Dowload', err.error.message, false);
                },
            });
        } else {
            this.startDownload();
        }
    }

    // download
    startDownload() {
        if (this.totalToDownload === null || this.totalToDownload <= 0) {
            showGlobalDialog("Total to download", "Must be positive integer", false);
            return;
        }
        this.updateLoading(true);
        this.videoService
            .startDownloadVideo({ totalToDownload: this.totalToDownload })
            .subscribe({
                next: (res: any) => {
                    if (res.data) {
                        this.updateLoading(false);
                    }
                    showGlobalDialog('Dowload', res.message, true);
                },
                error: (err) => {
                    showGlobalDialog('Dowload', err.error.message, false);
                },
            });
    }

    copyToClipboard(text: string) {
        navigator.clipboard
            .writeText(text)
            .then(() => {})
            .catch((err) => {});
    }

    updateLoading(loading: boolean) {
        this.isDownloading.update((x: boolean) => {
            return loading;
        });
        this.isLoading.update((x: boolean) => {
            return loading;
        });
    }

    // sheet dropdown
    toggleDropdown() {
        this.isOpenDropdown = !this.isOpenDropdown;
    }

    selectOption(sheet: any) {
        // this.sheetIndexSelected = option;
        this.seletectedSheet.update((x) => {
            return {
                sheetCode: sheet.sheetCode,
                sheetName: sheet.sheetName,
            };
        });
        this.isOpenDropdown = false;
    }
}
