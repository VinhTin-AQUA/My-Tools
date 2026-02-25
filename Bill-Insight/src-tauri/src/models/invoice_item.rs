use serde::{Deserialize, Serialize};

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct InvoiceItem {
    pub name: String,
    pub cash_price: f32,
    pub bank_price: f32,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ListInvoiceItems {
    pub items: Vec<InvoiceItem>,
    pub date: String,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct InvoiceExcel {
    pub invoice_date: String,
    pub name: String,
    pub cash: f32,
    pub bank: f32,
}
