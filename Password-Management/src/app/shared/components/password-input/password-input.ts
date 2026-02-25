import { Component, EventEmitter, forwardRef, Input, Output } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { Icon } from '../icon/icon';

@Component({
    selector: 'app-password-input',
    imports: [TranslatePipe, Icon],
    templateUrl: './password-input.html',
    styleUrl: './password-input.scss',
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => PasswordInput),
            multi: true,
        },
    ],
})
export class PasswordInput implements ControlValueAccessor {
    @Input() placeholder: string = '';
    @Input() label: string = '';
    @Input() readonly: boolean = false;
    @Input() disabled: boolean = false;

    @Output() onInput = new EventEmitter<any>();
    @Output() onChange = new EventEmitter<any>();

    value: string = '';
    showPassword = false;

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
