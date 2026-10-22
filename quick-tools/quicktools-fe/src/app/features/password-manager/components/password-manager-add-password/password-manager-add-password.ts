import { Component, effect, inject, input, output, signal, untracked } from '@angular/core';
import { FormField, form, required, submit, validate } from '@angular/forms/signals';
import { MessageService } from '@openng/optimus-ui/api';
import { ButtonModule } from '@openng/optimus-ui/button';
import { DialogModule } from '@openng/optimus-ui/dialog';
import { InputTextModule } from '@openng/optimus-ui/inputtext';
import { TextareaModule } from '@openng/optimus-ui/textarea';
import { AddPasswordManagerModel } from '../../models/password-manager.model';

interface AddPasswordFormValue {
    accountName: string;
    userName: string;
    password: string;
    confirmPassword: string;
    note: string;
}

@Component({
    selector: 'app-password-manager-add-password',
    imports: [ButtonModule, DialogModule, FormField, InputTextModule, TextareaModule],
    templateUrl: './password-manager-add-password.html',
    styleUrl: './password-manager-add-password.css',
})
export class PasswordManagerAddPassword {
    readonly visible = input(false);

    readonly visibleChange = output<boolean>();
    readonly passwordAdded = output<AddPasswordManagerModel>();

    private readonly messageService = inject(MessageService);

    readonly value = signal<AddPasswordFormValue>({
        accountName: '',
        userName: '',
        password: '',
        confirmPassword: '',
        note: '',
    });

    readonly addForm = form(this.value, (path) => {
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

            untracked(() => {
                this.value.set({
                    accountName: '',
                    userName: '',
                    password: '',
                    confirmPassword: '',
                    note: '',
                });
                this.addForm().reset();
            });
        });
    }

    protected close(): void {
        this.visibleChange.emit(false);
    }

    protected async onSubmit(event: Event): Promise<void> {
        event.preventDefault();

        const value = this.value();
        const password: AddPasswordManagerModel = {
            accountName: value.accountName.trim(),
            userName: value.userName.trim(),
            password: value.password,
            note: value.note.trim(),
        };

        console.log('Password added successfully.', password);

        this.messageService.add({
            severity: 'success',
            summary: 'Password added',
            detail: password.accountName,
        });

        this.passwordAdded.emit(password);
        this.close();
    }
}
