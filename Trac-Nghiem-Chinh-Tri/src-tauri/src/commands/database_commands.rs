use std::sync::Arc;

use tauri::{command, State};

use crate::states::AppState;

#[command]
pub async fn expot_json_data(state: State<'_, Arc<AppState>>) -> Result<bool, String> {
    let question_service = &state.db_service;

    let r = question_service
        .lock()
        .await
        .expot_json_data()
        .await
        .map_err(|e| e.to_string());

    r
}

#[command]
pub async fn import_json_to_db(
    state: State<'_, Arc<AppState>>,
    path: &str,
) -> Result<bool, String> {
    let question_service = &state.db_service;

    let r = question_service
        .lock()
        .await
        .import_json_to_db(path)
        .await
        .map_err(|e| e.to_string());

    r
}

#[command]
pub async fn delete_all_data(state: State<'_, Arc<AppState>>) -> Result<bool, String> {
    let question_service = &state.db_service;

    let r = question_service
        .lock()
        .await
        .delete_all_data()
        .await
        .map_err(|e| e.to_string());

    r
}
