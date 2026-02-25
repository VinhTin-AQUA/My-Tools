use serde::{Deserialize, Serialize};

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct SheetStats {
    pub used_cash: f32,
    pub used_bank: f32,
    pub total_cash: f32,
    pub total_bank: f32,
    pub remaining_cash: f32,
    pub remaining_bank: f32,
    pub total_remaining: f32,
}

#[derive(Deserialize, Debug)]
pub struct Spreadsheet {
    pub sheets: Vec<Sheet>,
}

#[derive(Deserialize, Debug)]
pub struct Sheet {
    pub properties: SheetProperties,
}

#[derive(Deserialize, Debug)]
pub struct SheetProperties {
    #[serde(rename = "sheetId")]
    pub sheet_id: i64,
    pub title: String,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct SheetInfo {
    pub sheet_id: i64,
    pub title: String,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct UpdateSheetInfo {
    pub sheet_id: i64,
    pub title: String,
}
