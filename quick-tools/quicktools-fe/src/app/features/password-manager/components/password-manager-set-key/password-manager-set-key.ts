import { Component, effect, inject, input, output, signal, untracked } from '@angular/core';
import { FormField, form, required, submit, validate } from '@angular/forms/signals';
import { MessageService } from '@openng/optimus-ui/api';
import { ButtonModule } from '@openng/optimus-ui/button';
import { DialogModule } from '@openng/optimus-ui/dialog';
import { InputTextModule } from '@openng/optimus-ui/inputtext';

interface SetKeyFormValue {
    oldKey: string;
    newKey: string;
    confirmNewKey: string;
}

@Component({
    selector: 'app-password-manager-set-key',
    imports: [ButtonModule, DialogModule, FormField, InputTextModule],
    templateUrl: './password-manager-set-key.html',
    styleUrl: './password-manager-set-key.css',
})
export class PasswordManagerSetKey {
    readonly visible = input(false);
    readonly existingKey = input<string | null>(null);

    readonly visibleChange = output<boolean>();
    readonly keySaved = output<string>();

    private readonly messageService = inject(MessageService);

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
        this.visibleChange.emit(false);
    }

    protected async onSubmit(event: Event): Promise<void> {
        event.preventDefault();

        await submit(this.setKeyForm, {
            action: async () => {
                console.log('Password manager key updated successfully.');

                this.messageService.add({
                    severity: 'success',
                    summary: 'Key saved',
                    detail: 'The password manager key was updated.',
                });

                this.keySaved.emit(this.value().newKey);
                this.close();

                return undefined;
            },
        });
    }
}
