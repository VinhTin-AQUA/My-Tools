import { signalStore, withState, withMethods, patchState } from '@ngrx/signals';
import { AccountModel } from '../../core/models/account-model';

const initialState: AccountModel = {
    id: '',
    account_name: '',
    user_name: '',
    password: '',
    note: '',
    salt_base64: '',
};

export const UpdateAccountStore = signalStore(
    {
        providedIn: 'root',
    },
    withState(initialState),
    withMethods((store) => {
        function update(model: AccountModel) {
            patchState(store, (currentState) => ({
                id: model.id,
                account_name: model.account_name,
                user_name: model.user_name,
                password: model.password,
                note: model.note,
                salt_base64: model.salt_base64,
            }));
        }

        return {
            update,
        };
    })
);
