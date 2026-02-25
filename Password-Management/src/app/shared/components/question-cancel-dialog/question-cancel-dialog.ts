import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { DialogService } from '../../services/dialog-service';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-question-cancel-dialog',
    imports: [TranslatePipe, CommonModule],
    templateUrl: './question-cancel-dialog.html',
    styleUrl: './question-cancel-dialog.scss',
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
