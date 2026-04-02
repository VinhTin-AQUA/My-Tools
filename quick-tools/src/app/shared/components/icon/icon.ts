import { CommonModule, NgComponentOutlet } from '@angular/common';
import { Component, Input, Type } from '@angular/core';
import { DownloadIconComponent, ErrorIconComponent, TickIconComponent, TrashIconComponent } from './icon.components';

export const ICON_REGISTRY: Record<string, Type<any>> = {
    download: DownloadIconComponent,
    tick: TickIconComponent,
    trash: TrashIconComponent,
    error: ErrorIconComponent,
};

export type IconNames = 'download' | 'tick' | 'trash' | 'error';

@Component({
    selector: 'app-icon',
    imports: [CommonModule, NgComponentOutlet],
    templateUrl: './icon.html',
    styleUrl: './icon.scss',
})
export class Icon {
    @Input() name: IconNames = 'error';

    iconComponent(): Type<any> | null {
        return ICON_REGISTRY[this.name] || null;
    }
}
