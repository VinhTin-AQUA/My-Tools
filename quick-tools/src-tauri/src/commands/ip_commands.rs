use crate::{
    models::ip_model::{IpAddress, IpAddressInfo},
    states::AppState,
};
use tauri::{command, State};
use tokio::sync::Mutex;

#[command]
pub async fn get_your_ip_command(state: State<'_, Mutex<AppState>>) -> Result<IpAddress, String> {
    let mut state_guard = state.lock().await;
    let ip_service = &mut state_guard.ip_service;

    // gọi hàm của service
    let r = ip_service
        .lock()
        .await
        .get_your_ip()
        .await
        .map_err(|e| e.to_string());

    r
}

#[command]
pub async fn get_your_ip_info_command(
    state: State<'_, Mutex<AppState>>,
    ip: &str,
) -> Result<IpAddressInfo, String> {
    let mut state_guard = state.lock().await;
    let ip_service = &mut state_guard.ip_service;

    // gọi hàm của service
    let r = ip_service
        .lock()
        .await
        .get_your_ip_info(ip)
        .await
        .map_err(|e| e.to_string());

    r
}
