import { Component, effect, inject, input, output, signal, untracked } from '@angular/core';
import { FormField, form, required, submit, validate } from '@angular/forms/signals';
import { MessageService } from '@openng/optimus-ui/api';
import { ButtonModule } from '@openng/optimus-ui/button';
import { DialogModule } from '@openng/optimus-ui/dialog';
import { InputTextModule } from '@openng/optimus-ui/inputtext';
import { TextareaModule } from '@openng/optimus-ui/textarea';
import { PasswordManagerModel } from '../../models/password-manager.model';

interface EditPasswordFormValue {
    accountName: string;
    userName: string;
    password: string;
    confirmPassword: string;
    note: string;
}

@Component({
    selector: 'app-password-manager-edit-password',
    imports: [ButtonModule, DialogModule, FormField, InputTextModule, TextareaModule],
    templateUrl: './password-manager-edit-password.html',
    styleUrl: './password-manager-edit-password.css',
})
export class PasswordManagerEditPassword {
    readonly visible = input(false);
    readonly password = input<PasswordManagerModel | null>(null);

    readonly visibleChange = output<boolean>();
    readonly passwordUpdated = output<PasswordManagerModel>();

    private readonly messageService = inject(MessageService);

    readonly value = signal<EditPasswordFormValue>({
        accountName: '',
        userName: '',
        password: '',
        confirmPassword: '',
        note: '',
    });

    readonly editForm = form(this.value, (path) => {
        required(path.accountName, { message: 'Account name is required.' });

        validate(path.accountName, ({ value }) =>
            value().trim()
                ? undefined
                : { kind: 'account-name-required', message: 'Account name is required.' },
        );

        required(path.userName, { message: 'Username is required.' });

        validate(path.userName, ({ value }) =>
            value().trim()
                ? undefined
                : { kind: 'user-name-required', message: 'Username is required.' },
        );

        required(path.password, { message: 'Password is required.' });

        validate(path.password, ({ value }) =>
            value().trim()
                ? undefined
                : { kind: 'password-required', message: 'Password is required.' },
        );

        required(path.confirmPassword, { message: 'Please confirm the password.' });

        validate(path.confirmPassword, ({ value, valueOf }) => {
            if (!value()) {
                return undefined;
            }

            return value() === valueOf(path.password)
                ? undefined
                : { kind: 'password-mismatch', message: 'Passwords do not match.' };
        });
    });

    constructor() {
        effect(() => {
            if (!this.visible()) {
                return;
            }

            const password = this.password();

            if (!password) {
                return;
            }

            untracked(() => {
                this.value.set({
                    accountName: password.accountName,
                    userName: password.userName,
                    password: password.password,
                    confirmPassword: password.password,
                    note: password.note,
                });
                this.editForm().reset();
            });
        });
    }

    protected close(): void {
        this.visibleChange.emit(false);
    }

    protected async onSubmit(event: Event): Promise<void> {
        event.preventDefault();

        await submit(this.editForm, {
            action: async () => {
                const password = this.password();

                if (!password) {
                    return undefined;
                }

                const value = this.value();
                const updatedPassword: PasswordManagerModel = {
                    id: password.id,
                    accountName: value.accountName.trim(),
                    userName: value.userName.trim(),
                    password: value.password,
                    note: value.note.trim(),
                };

                console.log('Password updated successfully.', {
                    id: updatedPassword.id,
                    accountName: updatedPassword.accountName,
                });

                this.messageService.add({
                    severity: 'success',
                    summary: 'Password updated',
                    detail: updatedPassword.accountName,
                });

                this.passwordUpdated.emit(updatedPassword);
                this.close();

                return undefined;
            },
        });
    }
}
