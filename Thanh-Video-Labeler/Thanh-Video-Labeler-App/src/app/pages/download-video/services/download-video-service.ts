import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment.development';

@Injectable({
    providedIn: 'root',
})
export class DownloadVideoService {
    readonly baseUrl = environment.baseUrl;

    constructor(private http: HttpClient) {}

    getRemainingVideos() {
        return this.http.get(`${this.baseUrl}/video/GetRemainingVideos`);
    }

    initVideo(model: { sheetName: string; sheetCode: string }) {
        return this.http.post(`${this.baseUrl}/video/InitDownloadVideo`, model);
    }

    startDownloadVideo(model: { totalToDownload: number }) {
        return this.http.post(`${this.baseUrl}/video/StartDownload`, model);
    }

    stopDownloadVideo() {
        return this.http.post(`${this.baseUrl}/video/StopDownload`, {});
    }
}
