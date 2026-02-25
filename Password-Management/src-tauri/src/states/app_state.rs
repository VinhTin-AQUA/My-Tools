use tokio::sync::Mutex;

use crate::services::GoogleSheetsService;

pub struct AppState {
    pub google_sheet_service: Mutex<GoogleSheetsService>,
}
