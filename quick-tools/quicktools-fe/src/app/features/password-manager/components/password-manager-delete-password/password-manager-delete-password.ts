import { Component, inject, input, output, signal } from '@angular/core';
import { MessageService } from '@openng/optimus-ui/api';
import { ButtonModule } from '@openng/optimus-ui/button';
import { GoogleSheetApiResult } from '../../../../models/google-sheet-api.model';
import { WebUIResponse } from '../../../../models/webui.response';
import { WebuiService } from '../../../../services/webui-service';
import { DeleteGoogleSheetPasswordManagerRequest } from './models/delete-googlesheet-password-manager.model';

@Component({
    selector: 'app-password-manager-delete-password',
    imports: [ButtonModule],
    templateUrl: './password-manager-delete-password.html',
    styleUrl: './password-manager-delete-password.css',
})
export class PasswordManagerDeletePassword {
    readonly passwordId = input<number | null>(null);

    readonly visibleChange = output<boolean>();
    readonly passwordDeleted = output<number>();

    private readonly messageService = inject(MessageService);
    private readonly webuiService = inject(WebuiService);

    readonly isDeleting = signal(false);

    protected close(): void {
        if (this.isDeleting()) {
            return;
        }

        this.visibleChange.emit(false);
    }

    protected async confirmDelete(): Promise<void> {
        const id = this.passwordId();

        if (id === null || this.isDeleting()) {
            return;
        }

        const request: DeleteGoogleSheetPasswordManagerRequest = { id };

        this.isDeleting.set(true);

        let result: WebUIResponse<GoogleSheetApiResult>;

        try {
            result = await this.webuiService.callJson<WebUIResponse<GoogleSheetApiResult>>(
                'GoogleSheetPasswordManagerHandler_Delete',
                request,
            );
        } finally {
            this.isDeleting.set(false);
        }

        if (!result.success || !result.data) {
            this.messageService.add({
                severity: 'error',
                summary: result.title || 'Delete failed',
                detail:
                    result.description ||
                    'The saved password could not be removed.',
            });

            return;
        }

        console.log('Password deleted successfully.', { id });

        this.messageService.add({
            severity: 'success',
            summary: 'Password deleted',
            detail: 'The saved password was removed.',
        });

        this.passwordDeleted.emit(id);
        this.close();
    }
}
