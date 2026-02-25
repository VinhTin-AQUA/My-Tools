import { Component, EventEmitter, forwardRef, Input, Output } from '@angular/core';
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
    selector: 'app-text-area-input',
    imports: [TranslatePipe],
    templateUrl: './text-area-input.html',
    styleUrl: './text-area-input.scss',
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => TextAreaInput),
            multi: true,
        },
    ],
})
export class TextAreaInput implements ControlValueAccessor {
    @Input() placeholder: string = '';
    @Input() label: string = '';
    @Input() readonly: boolean = false;
    @Input() disabled: boolean = false;

    @Output() onInput = new EventEmitter<any>();
    @Output() onChange = new EventEmitter<any>();

    value: string = '';

    // Callback từ Angular forms
    onChangeFn = (value: any) => {};
    onTouchedFn = () => {};

    // Ghi giá trị từ formControl vào component
    writeValue(value: any): void {
        this.value = value;
    }

    // Lưu callback khi value thay đổi
    registerOnChange(fn: any): void {
        this.onChangeFn = fn;
    }

    // Lưu callback khi control bị touch
    registerOnTouched(fn: any): void {
        this.onTouchedFn = fn;
    }

    // Khi input thay đổi
    handleInput(event: any) {
        const val = event.target.value;
        this.value = val;
        this.onChangeFn(val); // báo cho formControl
        this.onInput.emit(event); // sự kiện ngoài
    }

    handleChange(event: any) {
        this.onChange.emit(event); // sự kiện ngoài
    }

    handleFocus(event: any) {
        event.target.select();
        // setTimeout(() => event.target.select());
    }
}
