import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, switchMap, throwError } from 'rxjs';
import { IpApiResponse, IpifyResponse } from '../models/ip.model';

@Injectable({
    providedIn: 'root',
})
export class IpService {
    private readonly ipifyUrl = 'https://api.ipify.org/?format=json';
    private readonly ipApiUrl = 'http://ip-api.com/json';

    constructor(private http: HttpClient) {}

    getIpInformation(): Observable<IpApiResponse> {
        return this.http.get<IpifyResponse>(this.ipifyUrl).pipe(
            switchMap((ipInfo) => {
                if (!ipInfo || !ipInfo.ip) {
                    return throwError(() => new Error('Không lấy được Public IP.'));
                }

                const url = `${this.ipApiUrl}/${ipInfo.ip}`;

                return this.http.get<IpApiResponse>(url);
            }),
        );
    }
}
