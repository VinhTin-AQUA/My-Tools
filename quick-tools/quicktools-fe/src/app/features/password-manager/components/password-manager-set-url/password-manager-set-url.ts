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
    SetUrlGoogleSheetPasswordManagerRequest,
    SetUrlGoogleSheetPasswordManagerResponse,
} from './models/passwor-manager-set-key.model';

interface SetUrlFormValue {
    url: string;
}

@Component({
    selector: 'app-password-manager-set-url',
    imports: [ButtonModule, DialogModule, FormField, InputTextModule, BlockOutsideClose],
    templateUrl: './password-manager-set-url.html',
    styleUrl: './password-manager-set-url.css',
})
export class PasswordManagerSetUrl {
    readonly visible = input(false);
    readonly currentUrl = input<string | null>(null);

    readonly visibleChange = output<boolean>();
    readonly urlSaved = output<string>();

    private readonly messageService = inject(MessageService);
    private readonly webuiService = inject(WebuiService);

    readonly config = signal<GoogleSheetConfig | null>(null);
    readonly isSaving = signal(false);

    private readonly dialog = viewChild<Dialog>('dialog');

    readonly value = signal<SetUrlFormValue>({
        url: '',
    });

    readonly urlForm = form(this.value, (path) => {
        required(path.url, { message: 'URL is required.' });

        validate(path.url, ({ value }) =>
            value().trim() ? undefined : { kind: 'url-required', message: 'URL is required.' },
        );
    });

    constructor() {
        effect(() => {
            if (!this.visible()) {
                return;
            }

            const url = this.currentUrl() ?? '';

            untracked(() => {
                this.value.set({ url });
                this.urlForm().reset();
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
            const dialog = this.dialog();

            if (dialog) {
                dialog.visible = true;
            }

            return;
        }

        this.visibleChange.emit(visible);
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

        await submit(this.urlForm, {
            action: async () => {
                const request: SetUrlGoogleSheetPasswordManagerRequest = {
                    url: this.value().url.trim(),
                };

                this.isSaving.set(true);

                try {
                    await this.webuiService.callJson<SetUrlGoogleSheetPasswordManagerResponse>(
                        'GoogleSheetPasswordManagerConfigHandler_SetUrl',
                        request,
                    );
                } finally {
                    this.isSaving.set(false);
                }

                console.log('Password manager URL updated successfully.');

                this.messageService.add({
                    severity: 'success',
                    summary: 'URL saved',
                    detail: 'The password manager URL was updated.',
                });

                this.urlSaved.emit(request.url);
                this.close();

                return undefined;
            },
        });
    }
}
