import { Component, effect, inject, input, output, signal, untracked } from '@angular/core';
import { FormField, form, required, submit, validate } from '@angular/forms/signals';
import { MessageService } from '@openng/optimus-ui/api';
import { ButtonModule } from '@openng/optimus-ui/button';
import { DialogModule } from '@openng/optimus-ui/dialog';
import { InputTextModule } from '@openng/optimus-ui/inputtext';

interface SetUrlFormValue {
    url: string;
}

@Component({
    selector: 'app-password-manager-set-url',
    imports: [ButtonModule, DialogModule, FormField, InputTextModule],
    templateUrl: './password-manager-set-url.html',
    styleUrl: './password-manager-set-url.css',
})
export class PasswordManagerSetUrl {
    readonly visible = input(false);
    readonly currentUrl = input<string | null>(null);

    readonly visibleChange = output<boolean>();
    readonly urlSaved = output<string>();

    private readonly messageService = inject(MessageService);

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
        this.visibleChange.emit(false);
    }

    protected async onSubmit(event: Event): Promise<void> {
        event.preventDefault();

        await submit(this.urlForm, {
            action: async () => {
                const url = this.value().url.trim();

                console.log('Password manager URL updated successfully.');

                this.messageService.add({
                    severity: 'success',
                    summary: 'URL saved',
                    detail: 'The password manager URL was updated.',
                });

                this.urlSaved.emit(url);
                this.close();

                return undefined;
            },
        });
    }
}
