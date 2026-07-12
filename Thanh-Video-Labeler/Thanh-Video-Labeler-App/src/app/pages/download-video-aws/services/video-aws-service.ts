import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
    providedIn: 'root',
})
export class VideoAwsService {
    private readonly baseApi = environment.baseUrl;

    constructor(private http: HttpClient) {}

    uploadVideo(formData: FormData) {
        return this.http.post(
            `${this.baseApi}/VideoAws/UploadExcelAws`,
            formData
        );
    }

    getRemaining() {
        return this.http.get(`${this.baseApi}/VideoAws/GetRemaining`);
    }

    startDownload() {
        return this.http.post(`${this.baseApi}/VideoAws/DownloadVideoAws`, {});
    }

    stopDownload() {
        return this.http.post(`${this.baseApi}/VideoAws/StopDownload`, {});
    }
}
