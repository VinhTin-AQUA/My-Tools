import { patchState, signalStore, withMethods, withState } from '@ngrx/signals';

type AwsVideoDownloadingError = {
    case: string;
    serverId: string;
    link: string;
};

type AwsVideoDownloadingErrorTemp = {
    errors: AwsVideoDownloadingError[];
};

const initialState: AwsVideoDownloadingErrorTemp = {
    errors: []
};


export const AwsVideoDownlingErrorStore = signalStore(
    {
        providedIn: 'root',
    },
    withState(initialState),
    withMethods((store) => ({
        add(
           newState: AwsVideoDownloadingError
        ): void {
            patchState(store, (state) => {
                state.errors.push(newState);
                return state;
            });
        },
    }))
);
