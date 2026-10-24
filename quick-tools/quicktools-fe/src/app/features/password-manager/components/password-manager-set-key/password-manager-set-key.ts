import { Component, effect, inject, input, output, signal, untracked, viewChild } from '@angular/core';
import { FormField, form, required, submit, validate } from '@angular/forms/signals';
import { MessageService } from '@openng/optimus-ui/api';
import { ButtonModule } from '@openng/optimus-ui/button';
import { Dialog, DialogModule } from '@openng/optimus-ui/dialog';
import { InputTextModule } from '@openng/optimus-ui/inputtext';
import { GoogleSheetConfig } from '../../models/password-manager.model';
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

    readonly config = signal<GoogleSheetConfig | null>(null);

    readonly showOldKey = signal(false);
    readonly showNewKey = signal(false);
    readonly showConfirmNewKey = signal(false);
    readonly isSaving = signal(false);

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

    constructor() {
        effect(() => {
            if (!this.visible()) {
                return;
            }

            untracked(() => {
                this.value.set({
                    oldKey: '',
                    newKey: '',
                    confirmNewKey: '',
                });
                this.setKeyForm().reset();
            });
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
            this.dialog()!.visible = true;
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

    async ngOnInit(): Promise<void> {
        await this.loadConfig();
    }

    async loadConfig(): Promise<GoogleSheetConfig> {
        const config = await this.webuiService.callJson<GoogleSheetConfig>(
            'GoogleSheetPasswordManagerConfigHandler_GetConfig',
            {},
        );

        this.config.set(config);

        return config;
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

                try {
                    await this.webuiService.callJson<SetKeyGoogleSheetPasswordManagerResponse>(
                        'GoogleSheetPasswordManagerConfigHandler_SetKey',
                        request,
                    );
                } finally {
                    this.isSaving.set(false);
                }

                console.log('Password manager key updated successfully.');

                this.messageService.add({
                    severity: 'success',
                    summary: 'Key saved',
                    detail: 'The password manager key was updated.',
                });

                this.keySaved.emit(request.key);
                this.close();

                return undefined;
            },
        });
    }
}
