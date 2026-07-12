import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { UpdateVideo } from '../models/update-video';
import { ExportExcel } from '../models/export-excel';

@Injectable({
    providedIn: 'root',
})
export class VideoInforService {
    private baseUrl = environment.baseUrl;

    constructor(private http: HttpClient) {}

    getVideo(query: string) {
        return this.http.get(`${this.baseUrl}/video/Filter?${query}`);
    }

    update(model: UpdateVideo) {
        return this.http.post(`${this.baseUrl}/video/UpdateVideo`, model);
    }

    exportExcel(model: ExportExcel) {
        return this.http.post(`${this.baseUrl}/video/ExportExcel`, model, {
            responseType: 'blob',
        });
    }
}
