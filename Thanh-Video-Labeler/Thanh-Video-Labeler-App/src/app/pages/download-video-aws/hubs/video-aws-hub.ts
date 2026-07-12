import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment.development';
import * as signalR from '@microsoft/signalr';
import { AwsVideoDownlingStore } from '../signals/aws-video-downloading-signal';
import { AwsVideoDownlingErrorStore } from '../signals/aws-video-downloading-error-signal';
import { AwsVideoTotalSuccessStore } from '../signals/aws-video-total-success-signal';

@Injectable({
    providedIn: 'root',
})
export class VideoAwsHub {
    hubConnection?: signalR.HubConnection;
    videoDowloadHubUrl: string = environment.awsVideoDowloadHubloadHubUrl;
    awsVideoDownlingStore = inject(AwsVideoDownlingStore);

    awsVideoDownlingErrorStore = inject(AwsVideoDownlingErrorStore);
    awsVideoTotalSuccessStore = inject(AwsVideoTotalSuccessStore);

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

        this.hubConnection.on('RecieveProgress', (progress: string) => {
            console.log(progress);

            this.awsVideoDownlingStore.update(progress);
        });

        this.hubConnection.on(
            'RecieveDowloadingInfoVideo',
            (videoName: string) => {
                this.awsVideoDownlingStore.update(null, videoName);
            }
        );

        this.hubConnection.on(
            'RecieveErrorVideo',
            (_case: string, serverId: string, link: string) => {
                this.awsVideoDownlingErrorStore.add({
                    case: _case,
                    link: link,
                    serverId: serverId,
                });
            }
        );

        this.hubConnection.on(
            'RecieveIncreaseSucess',
            (totalSuccess: number) => {
                this.awsVideoTotalSuccessStore.increase(totalSuccess);
            }
        );

        //
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
