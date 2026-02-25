import { signalStore, withState, withMethods, patchState } from '@ngrx/signals';

type ToastState = {
    title: string;
    message: string;
    isSuccess: boolean;
};

const initialState: ToastState = {
    title: '',
    message: '',
    isSuccess: false,
};

export const ToastMessageStore = signalStore(
    {
        providedIn: 'root',
    },
    withState(initialState),
    withMethods((store) => {
        function update(model: ToastState) {
            patchState(store, (currentState) => ({
                title: model.title,
                message: model.message,
                isSuccess: model.isSuccess,
            }));
        }

        return {
            update,
        };
    })
);
