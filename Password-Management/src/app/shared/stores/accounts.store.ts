import { signalStore, withState, withMethods, patchState } from '@ngrx/signals';
import { AccountModel } from '../../core/models/account-model';
import { computed } from '@angular/core';

type AccountState = {
    accounts: AccountModel[];
};

const initialState: AccountState = {
    accounts: [],
};

export const AccountStore = signalStore(
    {
        providedIn: 'root',
    },
    withState(initialState),
    withMethods((store) => {
        function setAccounts(accounts: AccountModel[]) {
            patchState(store, (currentState) => ({
                accounts: accounts,
            }));
        }

        function addAccount(acc: AccountModel) {
            patchState(store, (currentState) => ({
                accounts: [...currentState.accounts, acc],
            }));
        }

        function delele(id: string) {
            patchState(store, (currentState) => ({
                accounts: store.accounts().filter((acc) => acc.id !== id),
            }));
        }

        function updateAccount(updated: AccountModel) {
            patchState(store, (currentState) => ({
                accounts: currentState.accounts.map((acc) =>
                    acc.id === updated.id ? { ...acc, ...updated } : acc
                ),
            }));
        }

        function getAccountById(id: string) {
            return computed(() => store.accounts().find((acc) => acc.id === id));
        }

        return { setAccounts, addAccount, delele, updateAccount, getAccountById };
    })
);
