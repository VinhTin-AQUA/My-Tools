import { patchState, signalStore, withMethods, withState } from '@ngrx/signals';

type AwsVideoTotalSuccess = {
    totalSuccess: number;
};

const initialState: AwsVideoTotalSuccess = {
    totalSuccess: 0,
};

export const AwsVideoTotalSuccessStore = signalStore(
    {
        providedIn: 'root',
    },
    withState(initialState),
    withMethods((store) => ({
        increase(totalSuccess: number): void {
            patchState(store, (state) => ({
                totalSuccess: state.totalSuccess + totalSuccess,
            }));
        },
    }))
);
