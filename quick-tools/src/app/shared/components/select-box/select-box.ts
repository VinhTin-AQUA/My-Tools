import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ClickOutsideDirective } from '../../directives/click-outside.directive';

export interface SelectOption {
    value: any;
    label: string;
    disabled?: boolean;
    icon?: string;
}

@Component({
    selector: 'app-select-box',
    imports: [ClickOutsideDirective],
    templateUrl: './select-box.html',
    styleUrl: './select-box.scss',
})
export class SelectBox {
    @Input() options: SelectOption[] = [];
    @Input() placeholder: string = 'Select an option';
    @Input() selectedValue: any;
    @Input() disabled: boolean = false;
    @Output() selectionChange = new EventEmitter<any>();

    isOpen = false;

    ngOnInit() {}

    get selectedOption(): SelectOption | undefined {
        return this.options.find((option) => option.value === this.selectedValue);
    }

    toggleDropdown(): void {
        if (!this.disabled) {
            this.isOpen = !this.isOpen;
        }
    }

    selectOption(option: SelectOption): void {
        if (!option.disabled) {
            this.selectedValue = option.value;
            this.selectionChange.emit(option.value);
            this.isOpen = false;
        }
    }

    closeDropdown(): void {
        this.isOpen = false;
    }
}
