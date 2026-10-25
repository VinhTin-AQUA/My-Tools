import { Component, effect, inject, input, output, signal, untracked, viewChild } from '@angular/core';
import { FormField, form, required, submit, validate } from '@angular/forms/signals';
import { MessageService } from '@openng/optimus-ui/api';
import { ButtonModule } from '@openng/optimus-ui/button';
import { Dialog, DialogModule } from '@openng/optimus-ui/dialog';
import { InputTextModule } from '@openng/optimus-ui/inputtext';
import { TextareaModule } from '@openng/optimus-ui/textarea';
import { PasswordManagerModel } from '../../models/password-manager.model';
import { GoogleSheetApiResult } from '../../../../models/google-sheet-api.model';
import { WebuiService } from '../../../../services/webui-service';
import { BlockOutsideClose } from '../../directives/block-outside-close.directive';

interface AddPasswordFormValue {
    accountName: string;
    userName: string;
    password: string;
    confirmPassword: string;
    note: string;
}

@Component({
    selector: 'app-password-manager-add-password',
    imports: [ButtonModule, DialogModule, FormField, InputTextModule, TextareaModule, BlockOutsideClose],
    templateUrl: './password-manager-add-password.html',
    styleUrl: './password-manager-add-password.css',
})
export class PasswordManagerAddPassword {
    readonly visible = input(false);

    readonly visibleChange = output<boolean>();
    readonly passwordAdded = output<PasswordManagerModel>();

    private readonly messageService = inject(MessageService);
    private readonly webuiService = inject(WebuiService);

    readonly showPassword = signal(false);
    readonly showConfirmPassword = signal(false);
    readonly isSaving = signal(false);

    private readonly dialog = viewChild<Dialog>('dialog');

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
                this.showPassword.set(false);
                this.showConfirmPassword.set(false);
            });
        });
    }

    protected togglePasswordVisibility(): void {
        this.showPassword.update((visible) => !visible);
    }

    protected toggleConfirmPasswordVisibility(): void {
        this.showConfirmPassword.update((visible) => !visible);
    }

    protected close(): void {
        if (this.isSaving()) {
            return;
        }

        this.visibleChange.emit(false);
    }

    protected onVisibleChange(visible: boolean): void {
        if (!visible && this.isSaving()) {
            const dialog = this.dialog();

            if (dialog) {
                dialog.visible = true;
            }

            return;
        }

        this.visibleChange.emit(visible);
    }

    protected async onSubmit(event: Event): Promise<void> {
        event.preventDefault();

        if (this.isSaving()) {
            return;
        }

        const value = this.value();
        const request: PasswordManagerModel = {
            id: 0,
            accountName: value.accountName.trim(),
            userName: value.userName.trim(),
            password: value.password,
            note: value.note.trim(),
        };

        const result = await this.savePassword(request);

        console.log("result:", result);
        

        const password: PasswordManagerModel = {
            ...request,
            id: result.id ?? request.id,
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

    private async savePassword(
        request: PasswordManagerModel,
    ): Promise<GoogleSheetApiResult> {
        this.isSaving.set(true);

        try {
            return await this.webuiService.callJson<GoogleSheetApiResult>(
                'GoogleSheetPasswordManagerHandler_Create',
                request,
            );
        } finally {
            this.isSaving.set(false);
        }
    }
}
