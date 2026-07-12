import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { DialogService } from '../../../core/services/dialog-service';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
    selector: 'app-question-cancel-dialog',
    imports: [CommonModule, TranslatePipe],
    templateUrl: './question-cancel-dialog.html',
    styleUrl: './question-cancel-dialog.css',
})
export class QuestionCancelDialog {
    @Input() title: string = 'Xác nhận';
    @Input() message: string = 'Bạn có chắc chắn?';
    @Input() isSuccess: boolean = false;

    @Output() close = new EventEmitter<boolean>();

    constructor(private dialogService: DialogService) {}

    onCancel() {
        this.close.emit(false);
    }

    onOK() {
        this.close.emit(true);
    }
}
