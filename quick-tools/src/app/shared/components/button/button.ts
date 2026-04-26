import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FieldTree } from '@angular/forms/signals';

type ButtonStatus =
    | 'primary'
    | 'success'
    | 'danger'
    | 'warning'
    | 'neutral'
    | 'outline-primary'
    | 'outline-success'
    | 'outline-danger'
    | 'outline-warning'
    | 'outline-neutral';

@Component({
    selector: 'app-button',
    imports: [CommonModule],
    templateUrl: './button.html',
    styleUrl: './button.scss',
})
export class Button {
    @Input() formField?: FieldTree<boolean>;
    @Input() disabled = false;
    @Input() icon?: 'check' | 'trash';
    @Input() status: ButtonStatus = 'primary';
    @Input() class = '';

    @Output() click = new EventEmitter<any>(); // <-- linh hoạt

    handleClick(event?: Event, payload?: any) {
        if (event) {
            event.stopPropagation();
        }
        if (!this.disabled) {
            this.click.emit(payload);
        }
    }
}
