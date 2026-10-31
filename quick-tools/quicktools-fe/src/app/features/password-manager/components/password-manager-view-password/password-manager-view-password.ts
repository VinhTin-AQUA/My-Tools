import { Component, computed, inject, input, output, signal } from '@angular/core';
import { MessageService } from '@openng/optimus-ui/api';
import { ButtonModule } from '@openng/optimus-ui/button';
import { InputTextModule } from '@openng/optimus-ui/inputtext';
import { WebUIResponse } from '../../../../models/webui.response';
import { WebuiService } from '../../../../services/webui-service';
import { PasswordManagerModel } from '../../models/password-manager.model';
import {
    VerifyKeyGoogleSheetPasswordManagerRequest,
    VerifyKeyGoogleSheetPasswordManagerResponse,
} from '../password-manager-set-key/models/passwor-manager-set-key.model';

interface ViewPasswordField {
    label: string;
    value: string;
}

@Component({
    selector: 'app-password-manager-view-password',
    imports: [ButtonModule, InputTextModule],
    templateUrl: './password-manager-view-password.html',
    styleUrl: './password-manager-view-password.css',
})
export class PasswordManagerViewPassword {
    readonly password = input<PasswordManagerModel | null>(null);

    readonly visibleChange = output<boolean>();

    private readonly messageService = inject(MessageService);
    private readonly webuiService = inject(WebuiService);

    readonly isVerified = signal(false);
    readonly showPassword = signal(false);
    readonly showVerifyKeyText = signal(false);
    readonly verifyKey = signal('');
    readonly isVerifying = signal(false);
    readonly verifyError = signal('');

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

    protected requestPasswordAccess(): void {
        if (!this.isVerified()) {
            return;
        }

        this.showPassword.update((visible) => !visible);
    }

    protected updateVerifyKey(event: Event): void {
        this.verifyKey.set((event.target as HTMLInputElement).value);
    }

    protected toggleVerifyKeyVisibility(): void {
        this.showVerifyKeyText.update((visible) => !visible);
    }

    protected async confirmVerifyKey(): Promise<void> {
        if (this.isVerifying()) {
            return;
        }

        const key = this.verifyKey().trim();

        if (!key) {
            this.verifyError.set('Please enter the key.');
            return;
        }

        this.isVerifying.set(true);

        try {
            const request: VerifyKeyGoogleSheetPasswordManagerRequest = { key };

            const response = await this.webuiService.callJson<
                WebUIResponse<VerifyKeyGoogleSheetPasswordManagerResponse>
            >('GoogleSheetPasswordManagerConfigHandler_VerifyKey', request);

            if (!response.success || !response.data?.isValid) {
                this.verifyError.set(response.description || 'The key is not valid.');
                return;
            }

            this.showVerifyKeyText.set(false);
            this.verifyKey.set('');
            this.verifyError.set('');
            this.isVerified.set(true);
        } catch {
            this.verifyError.set('Unable to verify the key.');
        } finally {
            this.isVerifying.set(false);
        }
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
