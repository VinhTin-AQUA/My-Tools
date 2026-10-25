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

    readonly isSaving = signal(false);
    readonly dialogMotionOptions = { enter: false, leave: false };

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

    private wasVisible = false;

    constructor() {
        effect(() => {
            const visible = this.visible();

            if (visible && !this.wasVisible) {
                untracked(() => {
                    const url = this.currentUrl() ?? '';

                    this.value.set({ url });
                    this.urlForm().reset();
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

                let response: WebUIResponse<SetUrlGoogleSheetPasswordManagerResponse>;

                try {
                    response = await this.webuiService.callJson<
                        WebUIResponse<SetUrlGoogleSheetPasswordManagerResponse>
                    >('GoogleSheetPasswordManagerConfigHandler_SetUrl', request);
                } finally {
                    this.isSaving.set(false);
                }

                if (!response.success) {
                    this.messageService.add({
                        severity: 'error',
                        summary: response.title || 'URL could not be saved',
                        detail:
                            response.description ||
                            'The password manager URL could not be saved.',
                    });

                    return undefined;
                }

                console.log('Password manager URL updated successfully.');

                this.messageService.add({
                    severity: 'success',
                    summary: 'URL saved',
                    detail: 'The password manager URL was updated.',
                });

                this.urlSaved.emit(response.data?.url ?? request.url);
                this.close();

                return undefined;
            },
        });
    }
}
