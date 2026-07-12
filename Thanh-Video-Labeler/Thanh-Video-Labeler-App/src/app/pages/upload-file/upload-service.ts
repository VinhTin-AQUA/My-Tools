import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';

@Injectable({
    providedIn: 'root',
})
export class UploadService {
    baseUrl = environment.baseUrl;

    constructor(private http: HttpClient) {}

    upload(file: File, isAccept: boolean) {
        const form = new FormData();
        form.append('file', file);
        form.append('isAccepted', isAccept == true ? 'true' : 'false');
        return this.http.post(`${this.baseUrl}/Upload/UploadExcel`, form);
    }
}
