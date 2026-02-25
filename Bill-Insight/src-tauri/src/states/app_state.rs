use tokio::sync::Mutex;

use crate::services::{BachHoaXanhService, GoogleSheetsService};

pub struct AppState {
    pub bhx_service: Mutex<BachHoaXanhService>,
    pub google_sheet_service: Mutex<GoogleSheetsService>,
}
