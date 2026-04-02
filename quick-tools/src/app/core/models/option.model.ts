export interface OptionModel<T = any> {
    label: string;
    value: T;
    disabled?: boolean;
    icon?: string;
}
