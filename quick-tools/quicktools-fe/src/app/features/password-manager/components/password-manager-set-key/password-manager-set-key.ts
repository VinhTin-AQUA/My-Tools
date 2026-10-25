import { Component, effect, inject, input, output, signal, untracked, viewChild } from '@angular/core';
import { FormField, form, required, submit, validate } from '@angular/forms/signals';
import { MessageService } from '@openng/optimus-ui/api';
import { ButtonModule } from '@openng/optimus-ui/button';
import { Dialog, DialogModule } from '@openng/optimus-ui/dialog';
import { InputTextModule } from '@openng/optimus-ui/inputtext';
import { WebUIResponse } from '../../../../models/webui.response';
import { WebuiService } from '../../../../services/webui-service';
import { BlockOutsideClose } from '../../directives/block-outside-close.directive';
import {
    SetKeyGoogleSheetPasswordManagerRequest,
    SetKeyGoogleSheetPasswordManagerResponse,
} from './models/passwor-manager-set-key.model';

interface SetKeyFormValue {
    oldKey: string;
    newKey: string;
    confirmNewKey: string;
}

@Component({
    selector: 'app-password-manager-set-key',
    imports: [ButtonModule, DialogModule, FormField, InputTextModule, BlockOutsideClose],
    templateUrl: './password-manager-set-key.html',
    styleUrl: './password-manager-set-key.css',
})
export class PasswordManagerSetKey {
    readonly visible = input(false);
    readonly existingKey = input<string | null>(null);

    readonly visibleChange = output<boolean>();
    readonly keySaved = output<string>();

    private readonly messageService = inject(MessageService);
    private readonly webuiService = inject(WebuiService);

    readonly showOldKey = signal(false);
    readonly showNewKey = signal(false);
    readonly showConfirmNewKey = signal(false);
    readonly isSaving = signal(false);
    readonly dialogMotionOptions = { enter: false, leave: false };

    private readonly dialog = viewChild<Dialog>('dialog');

    readonly value = signal<SetKeyFormValue>({
        oldKey: '',
        newKey: '',
        confirmNewKey: '',
    });

    readonly setKeyForm = form(this.value, (path) => {
        required(path.oldKey, {
            message: 'Old key is required.',
            when: () => !!this.existingKey(),
        });

        validate(path.oldKey, ({ value }) => {
            const existingKey = this.existingKey();

            if (!existingKey || !value()) {
                return undefined;
            }

            return value() === existingKey
                ? undefined
                : { kind: 'old-key-mismatch', message: 'Old key does not match.' };
        });

        required(path.newKey, { message: 'New key is required.' });

        validate(path.newKey, ({ value }) =>
            value().trim()
                ? undefined
                : { kind: 'new-key-required', message: 'New key is required.' },
        );

        required(path.confirmNewKey, { message: 'Please confirm the new key.' });

        validate(path.confirmNewKey, ({ value, valueOf }) => {
            if (!value()) {
                return undefined;
            }

            return value() === valueOf(path.newKey)
                ? undefined
                : { kind: 'new-key-mismatch', message: 'New keys do not match.' };
        });
    });

    private wasVisible = false;

    constructor() {
        effect(() => {
            const visible = this.visible();

            if (visible && !this.wasVisible) {
                untracked(() => {
                    this.value.set({
                        oldKey: '',
                        newKey: '',
                        confirmNewKey: '',
                    });
                    this.setKeyForm().reset();
                });
            }

            this.wasVisible = visible;
        });
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

    protected toggleOldKeyVisibility(): void {
        this.showOldKey.update((visible) => !visible);
    }

    protected toggleNewKeyVisibility(): void {
        this.showNewKey.update((visible) => !visible);
    }

    protected toggleConfirmNewKeyVisibility(): void {
        this.showConfirmNewKey.update((visible) => !visible);
    }

    protected async onSubmit(event: Event): Promise<void> {
        event.preventDefault();

        if (this.isSaving()) {
            return;
        }

        await submit(this.setKeyForm, {
            action: async () => {
                const request: SetKeyGoogleSheetPasswordManagerRequest = {
                    key: this.value().newKey,
                };

                this.isSaving.set(true);

                let response: WebUIResponse<SetKeyGoogleSheetPasswordManagerResponse>;

                try {
                    response = await this.webuiService.callJson<
                        WebUIResponse<SetKeyGoogleSheetPasswordManagerResponse>
                    >('GoogleSheetPasswordManagerConfigHandler_SetKey', request);
                } finally {
                    this.isSaving.set(false);
                }

                if (!response.success) {
                    this.messageService.add({
                        severity: 'error',
                        summary: response.title || 'Key could not be saved',
                        detail:
                            response.description ||
                            'The password manager key could not be saved.',
                    });

                    return undefined;
                }

                console.log('Password manager key updated successfully.');

                this.messageService.add({
                    severity: 'success',
                    summary: 'Key saved',
                    detail: 'The password manager key was updated.',
                });

                this.keySaved.emit(response.data?.key ?? request.key);
                this.close();

                return undefined;
            },
        });
    }
}
