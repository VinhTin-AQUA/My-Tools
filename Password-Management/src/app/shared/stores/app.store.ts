import { signalStore, withState, withMethods, patchState } from '@ngrx/signals';

type AppState = {
    passCode: string;
    theme: string;
};

const initialState: AppState = {
    passCode: '',
    theme: ''
};

export const AppStore = signalStore(
    {
        providedIn: 'root',
    },
    withState(initialState),
    withMethods((store) => {
        function updatePasscode(passCode: string) {
            patchState(store, (currentState) => ({
                passCode: passCode,
            }));
        }

        function updateTheme(theme: string) {
            patchState(store, (currentState) => ({
                theme: theme
            }));
        }

        return { updatePasscode, updateTheme };
    })
);
