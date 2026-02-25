export interface InvoiceItem {
    name: string;
    cash_price: number;
    bank_price: number;
}

export interface ListInvoiceItems {
    date: string; // "2025-10-25"
    expanded: boolean;
    items: InvoiceItem[];
}
