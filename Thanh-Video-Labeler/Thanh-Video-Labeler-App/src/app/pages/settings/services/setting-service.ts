import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment.development';

@Injectable({
    providedIn: 'root',
})
export class SettingService {
    baseUrl = environment.baseUrl;
    constructor(private http: HttpClient) {}

    getSettingInfo() {
        return this.http.get(`${this.baseUrl}/setting/GetSettingInfo`);
    }

    resetData() {
        return this.http.post(`${this.baseUrl}/setting/ResetData`, {});
    }
}
