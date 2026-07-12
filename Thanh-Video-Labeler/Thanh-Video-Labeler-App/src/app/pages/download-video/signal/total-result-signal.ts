import { signal } from '@angular/core';
import { ResultDownloadVideo } from '../models/result-download-video';

type DownloadVideoError = {
    transId: string;
    link: string;
};

type TotalResultState = {
    totalDownloadsSuccess: number;
    totalDownloadsFailed: number;
    downloadVideoErrors: DownloadVideoError[];
};

let initState: TotalResultState = {
    totalDownloadsFailed: 0,
    totalDownloadsSuccess: 0,
    downloadVideoErrors: [
        // {
        //     link: 'https://s3.amazonaws.com/i3.wholefoods/CMSWeb/KDVRs_355/89992b51-2378-41a0-a434-361d24cf7df4/2012002_C15_Customer_Service_20250509_194247/2012002_C15_Customer_Service_20250509_194247.mp4',
        //     transId: '790636069',
        // },
    ],
};

export const resultDownloadVideoSignal = signal<TotalResultState>(initState);

export function updateDownloadVideoResult(result: ResultDownloadVideo) {
    resultDownloadVideoSignal.update((x: TotalResultState) => {
        if (result.isSuccess) {
            x.totalDownloadsSuccess =
                x.totalDownloadsSuccess + result.totalDownloadsSuccess;
        } else {
            x.totalDownloadsFailed =
                x.totalDownloadsFailed + result.totalDownloadsFailed;
            x.downloadVideoErrors.push({
                link: result.link,
                transId: result.transId,
            });
        }

        return {
            totalDownloadsFailed: x.totalDownloadsFailed,
            totalDownloadsSuccess: x.totalDownloadsSuccess,
            downloadVideoErrors: x.downloadVideoErrors,
        };
    });
}
