import { Component, inject, input, output } from '@angular/core';
import { MessageService } from '@openng/optimus-ui/api';
import { ButtonModule } from '@openng/optimus-ui/button';
import { DialogModule } from '@openng/optimus-ui/dialog';

@Component({
    selector: 'app-password-manager-delete-password',
    imports: [ButtonModule, DialogModule],
    templateUrl: './password-manager-delete-password.html',
    styleUrl: './password-manager-delete-password.css',
})
export class PasswordManagerDeletePassword {
    readonly visible = input(false);
    readonly passwordId = input<number | null>(null);

    readonly visibleChange = output<boolean>();
    readonly passwordDeleted = output<number>();

    private readonly messageService = inject(MessageService);

    protected close(): void {
        this.visibleChange.emit(false);
    }

    protected confirmDelete(): void {
        const id = this.passwordId();

        if (id === null) {
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
