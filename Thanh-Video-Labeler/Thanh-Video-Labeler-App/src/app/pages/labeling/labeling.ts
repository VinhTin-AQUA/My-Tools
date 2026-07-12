import { ChangeDetectorRef, Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ConvertToQueryString } from '../../helper/convertToQueryString';
import { VideoInforService } from './services/video-infor-service';
import { VideoInfo } from './models/videoInfo';
import { LabelingDetails } from '../labeling-details/labeling-details';
import {
    hideGlobalLoading,
    showGlobalLoading,
} from '../../shared/signals/loading-signal';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
    selector: 'app-labeling',
    imports: [FormsModule, LabelingDetails],
    templateUrl: './labeling.html',
    styleUrl: './labeling.scss',
})
export class Labeling {
    filters = {
        transIdOrPayment: '',
        label: false,
        noLabel: false,
        page: 1,
        pageSize: 20,
    };

    showPopup = false;
    clearAllData = false;

    videos = signal<VideoInfo[]>([]);
    selectedVideo: VideoInfo | null = null;
    selectedIndex: number = 0;

    constructor(
        private videoInforService: VideoInforService,
        private activatedRoute: ActivatedRoute,
        private router: Router,
        private cdr: ChangeDetectorRef
    ) {}

    ngOnInit() {
        this.activatedRoute.queryParams.subscribe((res: any) => {
            if (res.page) {
                this.filters.page = res.page;
            }

            if (res.pageSize) {
                this.filters.pageSize = res.pageSize;
            }
            this.cdr.detectChanges();
            this.filter(false);
        });
    }

    private filter(canShowVideo: boolean) {
        this.videoInforService
            .getVideo(ConvertToQueryString(this.filters))
            .subscribe({
                next: (res: any) => {
                    this.videos.update((x) => res.data);

                    if (canShowVideo) {
                        this.selectedVideo = this.videos()[0];
                    }
                },
                error: (err) => {
                    console.log(err.error);
                },
            });
    }

    applyFilter() {
        this.filter(false);
    }

    nextPage() {
        this.filters.page++;
        this.router.navigate([], {
            relativeTo: this.activatedRoute,
            queryParams: { page: this.filters.page },
            queryParamsHandling: 'merge', // giữ các query params khác
        });
    }

    prevPage() {
        if (this.filters.page > 1) {
            this.filters.page--;

            this.router.navigate([], {
                relativeTo: this.activatedRoute,
                queryParams: { page: this.filters.page },
                queryParamsHandling: 'merge', // giữ các query params khác
            });
        }
    }

    openVideoModal(video: VideoInfo) {
        this.selectedVideo = video;
        const index = this.videos().findIndex(
            (item) => item.transID === video.transID
        );
        this.selectedIndex = index;

        console.log(this.selectedIndex);

        if (this.selectedVideo) {
            document.body.classList.add('overflow-hidden');
        } else {
            document.body.classList.remove('overflow-hidden');
        }
    }

    closeModal() {
        this.selectedVideo = null;
        document.body.classList.remove('overflow-hidden');
    }

    // export excel
    openExportExcelPopup() {
        this.showPopup = true;
    }

    closeExportExcelPopup() {
        this.showPopup = false;
        this.clearAllData = false;
    }

    downloadExcel() {
        console.log(this.clearAllData);
        showGlobalLoading();
        this.videoInforService
            .exportExcel({ clearAllData: this.clearAllData })
            .subscribe({
                next(value) {
                    const fileName = 'Export.xlsx';
                    const url = window.URL.createObjectURL(value);
                    const a = document.createElement('a');
                    a.href = url;
                    a.download = fileName;
                    a.click();
                    window.URL.revokeObjectURL(url); // dọn dẹp
                    hideGlobalLoading();
                },
                error(err) {
                    hideGlobalLoading();
                    console.error('Lỗi khi tải file:', err);
                },
            });
    }

    onNext() {
        if (this.selectedIndex < this.videos().length - 1) {
            this.selectedIndex++;
            this.selectedVideo = this.videos()[this.selectedIndex];
            console.log(this.selectedVideo.transID);
        } else {
            this.selectedIndex = 0;
            this.filters.page++;
            this.filter(true);
        }
    }

    onPrev() {
        if (this.selectedIndex > 0) {
            this.selectedIndex--;
            this.selectedVideo = this.videos()[this.selectedIndex];
            console.log(this.selectedVideo.transID);
        }
    }
}
