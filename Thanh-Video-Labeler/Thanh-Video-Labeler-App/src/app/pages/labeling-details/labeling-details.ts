import {
    Component,
    ElementRef,
    EventEmitter,
    Input,
    Output,
    SimpleChanges,
    ViewChild,
} from '@angular/core';
import { VideoInfo } from '../labeling/models/videoInfo';
import { FormsModule } from '@angular/forms';
import { VideoInforService } from '../labeling/services/video-infor-service';
import { showGlobalDialog } from '../../shared/signals/dialog-signal';
import { environment } from '../../../environments/environment.development';

@Component({
    selector: 'app-labeling-details',
    imports: [FormsModule],
    templateUrl: './labeling-details.html',
    styleUrl: './labeling-details.scss',
})
export class LabelingDetails {
    @Input() videoInfo!: VideoInfo;
    videoUrl = environment.videoBaseUrl + '/VideoFolder';
    @Output() nextVideo = new EventEmitter<void>();
    @Output() prevVideo = new EventEmitter<void>();
    @ViewChild('videoPlayer') videoPlayer!: ElementRef<HTMLVideoElement>;

    constructor(private videoInforService: VideoInforService) {}

    update() {
        this.videoInforService
            .update({ id: this.videoInfo.id, label: this.videoInfo.label })
            .subscribe({
                next: (res: any) => {
                    showGlobalDialog('Update', res.message, true);
                },
                error: (err: any) => {
                    showGlobalDialog('Update', err.error.message, false);
                },
            });
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (!changes['videoInfo'].previousValue) {
            return;
        }
        const currentTransID = changes['videoInfo'].currentValue.transID;
        const previousTransID = changes['videoInfo'].previousValue.transID;

        if (currentTransID !== previousTransID && this.videoPlayer) {
            // Gọi load() để reload video
            this.videoPlayer.nativeElement.load();
        }
    }

    onNextClick() {
        this.nextVideo.emit();
    }

    onPrevClick() {
        this.prevVideo.emit();
    }
}
