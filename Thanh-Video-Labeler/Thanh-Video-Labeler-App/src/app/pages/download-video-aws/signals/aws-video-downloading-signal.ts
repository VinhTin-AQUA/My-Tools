import { patchState, signalStore, withMethods, withState } from '@ngrx/signals';

type AwsVideoDownloading = {
    progress: string;
    videoName: string;
};

const initialState: AwsVideoDownloading = {
    progress: '0 MB / 0 MB',
    videoName: 'no video',
};

export const AwsVideoDownlingStore = signalStore(
    {
        providedIn: 'root',
    },
    withState(initialState),
    withMethods((store) => ({
        update(
            progress: string | null = null,
            videoName: string | null = null
        ): void {
            patchState(store, (state) => ({
                progress: progress ?? state.progress,
                videoName: videoName ?? state.videoName,
            }));
        },
    }))
);
