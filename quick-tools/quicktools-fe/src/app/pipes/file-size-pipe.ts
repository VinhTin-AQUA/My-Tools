import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'fileSize',
})
export class FileSizePipe implements PipeTransform {
    transform(bytes: number): string {
        if (bytes === 0) return '0 B';

        const k = 1024;
        const sizes = ['B', 'KB', 'MB', 'GB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        const size = bytes / Math.pow(k, i);

        if (i === 0) return `${size} ${sizes[i]}`;
        if (i === 1) return `${Math.round(size)} ${sizes[i]}`;

        return `${size.toFixed(1)} ${sizes[i]}`;
    }
}
