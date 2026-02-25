export interface InvoiceExcel {
    invoice_date: string,
    name: string,
    cash: number,
    bank: number,
}

export interface CashItem {
    id: string;
    name: string,
    cash: number,
}