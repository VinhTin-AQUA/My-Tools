import { Component, EventEmitter, forwardRef, Input, Output } from '@angular/core';
import { NG_VALUE_ACCESSOR, ControlValueAccessor, FormsModule } from '@angular/forms';
import { FormField, FieldTree } from '@angular/forms/signals';
import { OptionModel } from '../../../core/models/option.model';

@Component({
    selector: 'app-select-box',
    imports: [FormField, FormsModule],
    templateUrl: './select-box.html',
    styleUrl: './select-box.scss',
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => SelectBox),
            multi: true,
        },
    ],
})
export class SelectBox implements ControlValueAccessor {
    @Input() formField?: FieldTree<string>;
    @Input() name!: string;
    @Input() label: string = '';
    @Input() description: string = '';
    @Input() value!: string;
    @Input() disabled: boolean = false;

    @Input() options: OptionModel<string>[] = [];
    @Input() optionLabel: string = 'label';
    @Input() optionValue: string = 'value';

    @Output() valueChange = new EventEmitter<string>();

    // ControlValueAccessor methods
    onChange = (value: any) => {};
    onTouched = () => {};

    writeValue(value: any): void {
        this.value = value ?? '';
    }

    registerOnChange(fn: any): void {
        this.onChange = fn;
    }

    registerOnTouched(fn: any): void {
        this.onTouched = fn;
    }

    setDisabledState(isDisabled: boolean): void {
        this.disabled = isDisabled;
    }

    handleChange(event: any) {
        const selectedValue = event;
        this.value = selectedValue;

        if (this.formField) {
            this.formField()!.controlValue.set(selectedValue);
        }

        this.onChange(selectedValue);
        this.onTouched();
        this.valueChange.emit(selectedValue);
    }
}
