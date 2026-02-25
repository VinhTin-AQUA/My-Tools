import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
    selector: 'app-file-input',
    imports: [TranslatePipe],
    templateUrl: './file-input.html',
    styleUrl: './file-input.scss',
})
export class FileInput {
    @Input() label: string = '';
    @Input() accept: string = '';

    @Input() placeholder: string = '';
    @Input() readonly: boolean = false;
    @Input() disabled: boolean = false;

    @Output() onInput = new EventEmitter<any>();
    @Output() onChange = new EventEmitter<any>();

    handleInput(event: any) {
        this.onInput.emit(event);
    }

    handleChange(event: any) {
        this.onChange.emit(event);
    }
}
