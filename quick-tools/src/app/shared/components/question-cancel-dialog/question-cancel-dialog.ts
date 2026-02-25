import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { QuestionCancelDialogStore } from '../../stores/question-cancel-dialog.store';
import { TranslatePipe } from '@ngx-translate/core';
import { DialogService } from '../../../core/services/dialog-service';

@Component({
    selector: 'app-question-cancel-dialog',
    imports: [TranslatePipe],
    templateUrl: './question-cancel-dialog.html',
    styleUrl: './question-cancel-dialog.scss',
})
export class QuestionCancelDialog {
    questionCancelDialogStore = inject(QuestionCancelDialogStore);

    @Output() confirm = new EventEmitter<void>();

    constructor(private dialogService: DialogService) {}

    onConfirm(): void {
        this.confirm.emit();
        this.close();
    }

    onCancel(): void {
        this.dialogService.showQuestionCancelDialog(false, '', '', true);
    }

    close(): void {
        this.dialogService.showQuestionCancelDialog(false, '', '', true);
    }
}
