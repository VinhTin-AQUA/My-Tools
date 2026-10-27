import { Component, computed, inject, input, output, signal } from '@angular/core';
import { MessageService } from '@openng/optimus-ui/api';
import { ButtonModule } from '@openng/optimus-ui/button';
import { PasswordManagerModel } from '../../models/password-manager.model';

interface ViewPasswordField {
    label: string;
    value: string;
}

@Component({
    selector: 'app-password-manager-view-password',
    imports: [ButtonModule],
    templateUrl: './password-manager-view-password.html',
    styleUrl: './password-manager-view-password.css',
})
export class PasswordManagerViewPassword {
    readonly password = input<PasswordManagerModel | null>(null);

    readonly visibleChange = output<boolean>();

    private readonly messageService = inject(MessageService);

    readonly showPassword = signal(false);

    readonly fields = computed<ViewPasswordField[]>(() => {
        const password = this.password();

        if (!password) {
            return [];
        }

        return [
            { label: 'ID', value: String(password.id) },
            { label: 'Account Name', value: password.accountName },
            { label: 'Username', value: password.userName },
            { label: 'Password', value: password.password },
            { label: 'Note', value: password.note },
        ];
    });

    protected displayValue(field: ViewPasswordField): string {
        if (!field.value) {
            return '—';
        }

        if (field.label === 'Password' && !this.showPassword()) {
            return '••••••••';
        }

        return field.value;
    }

    protected togglePasswordVisibility(): void {
        this.showPassword.update((visible) => !visible);
    }

    protected close(): void {
        this.visibleChange.emit(false);
    }

    protected async copyValue(field: ViewPasswordField): Promise<void> {
        try {
            await navigator.clipboard.writeText(field.value);

            this.messageService.add({
                severity: 'success',
                summary: 'Copied',
                detail: `${field.label} copied to clipboard.`,
            });
        } catch {
            this.messageService.add({
                severity: 'error',
                summary: 'Copy failed',
                detail: `Unable to copy ${field.label.toLowerCase()}.`,
            });
        }
    }
}
