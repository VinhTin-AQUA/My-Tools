use crate::models::{
    InvoiceExcel, ListInvoiceItems, ResponseCommand, SheetInfo, SheetStats, UpdateSheetInfo,
};
use crate::services::TokenResponse;
use crate::states::AppState;
use tauri::{command, State};
use tokio::sync::Mutex;

#[command]
pub async fn init_google_sheet_command(
    state: State<'_, Mutex<AppState>>,
    json_path: String,
) -> Result<Option<TokenResponse>, String> {
    let mut state_guard = state.lock().await;
    let google_service = &mut state_guard.google_sheet_service;
    let t: Result<Option<TokenResponse>, String> = google_service
        .lock()
        .await
        .init_google_service(&json_path)
        .await
        .map_err(|e| e.to_string());
    t
}

#[command]
pub async fn get_invoices(
    state: State<'_, Mutex<AppState>>,
    sheet_name: String,
    spreadsheet_id: String,
) -> Result<Vec<ListInvoiceItems>, String> {
    let mut state_guard = state.lock().await;
    let google_service = &mut state_guard.google_sheet_service;

    let r = google_service
        .lock()
        .await
        .get_invoices(sheet_name, spreadsheet_id)
        .await
        .map_err(|e| e.to_string());
    r
}

#[command]
pub async fn get_sheet_stats(
    state: State<'_, Mutex<AppState>>,
    sheet_name: String,
    spreadsheet_id: String,
) -> Result<SheetStats, String> {
    let mut state_guard = state.lock().await;
    let google_service = &mut state_guard.google_sheet_service;

    let r = google_service
        .lock()
        .await
        .get_sheet_stats(sheet_name, spreadsheet_id)
        .await
        .map_err(|e| e.to_string());
    r
}

#[command]
pub async fn set_invoices(
    state: State<'_, Mutex<AppState>>,
    sheet_name: String,
    spreadsheet_id: String,
    items: Vec<InvoiceExcel>,
) -> Result<ResponseCommand, String> {
    let mut state_guard = state.lock().await;
    let google_service = &mut state_guard.google_sheet_service;

    let r = google_service
        .lock()
        .await
        .set_invoices(sheet_name, spreadsheet_id, items)
        .await
        .map_err(|e| e.to_string());
    r
}

#[command]
pub async fn list_sheets(
    state: State<'_, Mutex<AppState>>,
    spreadsheet_id: String,
) -> Result<Vec<SheetInfo>, String> {
    let mut state_guard = state.lock().await;
    let google_service = &mut state_guard.google_sheet_service;

    let r = google_service
        .lock()
        .await
        .list_sheets(spreadsheet_id)
        .await
        .map_err(|e| e.to_string());
    r
}

#[command]
pub async fn update_sheet_name(
    state: State<'_, Mutex<AppState>>,
    update_sheet: UpdateSheetInfo,
    spreadsheet_id: String,
) -> Result<Option<bool>, String> {
    let mut state_guard = state.lock().await;
    let google_service = &mut state_guard.google_sheet_service;

    let r = google_service
        .lock()
        .await
        .update_sheet_name(update_sheet, spreadsheet_id)
        .await
        .map_err(|e| e.to_string());
    r
}
