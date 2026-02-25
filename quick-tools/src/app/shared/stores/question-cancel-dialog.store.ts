import { patchState, signalStore, withMethods, withState } from '@ngrx/signals';

type QuestionCancelDialogState = {
    title: string;
    message: string;
    isSuccess: boolean;
};

const initialState: QuestionCancelDialogState = {
    title: '',
    message: '',
    isSuccess: false,
};

export const QuestionCancelDialogStore = signalStore(
    {
        providedIn: 'root',
    },
    withState(initialState),
    withMethods((store) => {
        function update(title: string = '', message: string = '', isSuccess: boolean = false) {
            patchState(store, (currentState) => ({
                title: title,
                message: message,
                isSuccess: isSuccess,
            }));
        }

        return {
            update,
        };
    })
);
