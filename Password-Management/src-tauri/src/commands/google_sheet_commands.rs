use crate::models::{Password, ResponseCommand};
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
pub async fn add_account(
    state: State<'_, Mutex<AppState>>,
    sheet_name: String,
    spreadsheet_id: String,
    passcode: String,
    password: Password,
) -> Result<Option<ResponseCommand>, String> {
    let mut state_guard = state.lock().await;
    let google_service = &mut state_guard.google_sheet_service;

    let t: Result<Option<ResponseCommand>, String> = google_service
        .lock()
        .await
        .add_account(sheet_name, spreadsheet_id, passcode, password)
        .await
        .map_err(|e| e.to_string());
    t
}

#[command]
pub async fn get_accounts(
    state: State<'_, Mutex<AppState>>,
    sheet_name: String,
    spreadsheet_id: String,
    passcode: String,
) -> Result<Option<Vec<Password>>, String> {
    let mut state_guard = state.lock().await;
    let google_service = &mut state_guard.google_sheet_service;

    let t: Result<Option<Vec<Password>>, String> = google_service
        .lock()
        .await
        .get_accounts(sheet_name, spreadsheet_id, passcode)
        .await
        .map_err(|e| e.to_string());
    t
}

#[command]
pub async fn delete_account(
    state: State<'_, Mutex<AppState>>,
    sheet_name: String,
    spreadsheet_id: String,
    id: String,
) -> Result<Option<bool>, String> {
    let mut state_guard = state.lock().await;
    let google_service = &mut state_guard.google_sheet_service;

    let t: Result<Option<bool>, String> = google_service
        .lock()
        .await
        .delete_account(sheet_name, spreadsheet_id, id)
        .await
        .map_err(|e| e.to_string());
    t
}

#[command]
pub async fn update_account(
    state: State<'_, Mutex<AppState>>,
    sheet_name: String,
    spreadsheet_id: String,
    passcode: String,
    password: Password,
) -> Result<Option<ResponseCommand>, String> {
    let mut state_guard = state.lock().await;
    let google_service = &mut state_guard.google_sheet_service;

    let t: Result<Option<ResponseCommand>, String> = google_service
        .lock()
        .await
        .update_account(sheet_name, spreadsheet_id, passcode, password)
        .await
        .map_err(|e| e.to_string());
    t
}
