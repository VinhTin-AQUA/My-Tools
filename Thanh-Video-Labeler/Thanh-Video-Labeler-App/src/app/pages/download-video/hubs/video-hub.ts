import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../../environments/environment.development';
import { ResultDownloadVideo } from '../models/result-download-video';
import { updateDownloadVideoResult } from '../signal/total-result-signal';

@Injectable({
    providedIn: 'root',
})
export class VideoHub {
    hubConnection?: signalR.HubConnection;
    public receivedMessage: string = '';
    videoDowloadHubUrl: string = environment.videoDowloadHubUrl;

    constructor() {}

    public startConnection(): void {
        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl(this.videoDowloadHubUrl) // đổi URL theo API của bạn
            .withAutomaticReconnect()
            .build();

        this.hubConnection
            .start()
            .then(() => console.log('SignalR connection started'))
            .catch((err) =>
                console.log('Error while starting SignalR connection: ' + err)
            );

        this.hubConnection.on(
            'RecieveResultDownloadVideo',
            (result: ResultDownloadVideo) => {
                updateDownloadVideoResult(result);
            }
        );
    }

    public sendMessage(user: string, message: string): void {
        if (this.hubConnection) {
            this.hubConnection
                .invoke('SendMessage', user, message)
                .catch((err) => console.error(err));
        }
    }

    public stopConnection(): void {
        if (this.hubConnection) {
            this.hubConnection.stop().then(() => {
                console.log('SignalR connection stopped.');
            });
        }
    }
}
