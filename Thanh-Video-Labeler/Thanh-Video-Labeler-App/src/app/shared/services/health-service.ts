import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';

@Injectable({
    providedIn: 'root',
})
export class HealthService {
    baseUrl = environment.baseUrl;

    constructor(private http: HttpClient) {}

    ping() {
        return this.http.get(`${this.baseUrl}/Health/ping`);
    }
}
