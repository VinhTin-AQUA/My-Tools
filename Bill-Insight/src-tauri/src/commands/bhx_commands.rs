use std::sync::Arc;

use crate::models::{CookieModel, HHDVu, NBan, ReadXmlDataResult, TToan};
use crate::services::BachHoaXanhService;
use crate::states::AppState;
use tauri::{command, State};
use tokio::sync::Mutex;

#[command]
pub async fn get_captcha_and_asp_session(
    state: State<'_, Mutex<AppState>>,
    folder: String,
) -> Result<Option<CookieModel>, String> {
    let mut state_guard = state.lock().await;
    let bhx_service = &mut state_guard.bhx_service;

    let r = bhx_service
        .lock()
        .await
        .get_captcha_and_asp_session(folder)
        .await
        .map_err(|e| e.to_string());

    r
}

#[command]
pub async fn get_xml_invoice_data(
    state: State<'_, Mutex<AppState>>,
    sv_id: &str,
    asp_session: &str,
    captcha: &str,
    phone: &str,
    invoice_num: &str,
    folder: &str,
) -> Result<Option<ReadXmlDataResult>, String> {
    let mut state_guard = state.lock().await;
    let bhx_service = &mut state_guard.bhx_service;

    let r = bhx_service
        .lock()
        .await
        .get_xml_invoice_data(sv_id, asp_session, captcha, phone, invoice_num, folder)
        .await
        .map_err(|e| e.to_string());
    r
}
