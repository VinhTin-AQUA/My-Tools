use crate::states::AppState;
use tauri::{command, State};
use tokio::sync::Mutex;


use i_love_img_upscale_img_lib::{BinaryFile, UpscaleResult};

#[command]
pub async fn iloveimg_upscale_img_command(
    state: State<'_, Mutex<AppState>>,
    files: Vec<BinaryFile>,
) -> Result<Vec<UpscaleResult>, String> {
    let mut state_guard = state.lock().await;
    let iloveimg_upscale_img_service = &mut state_guard.iloveimg_upscale_img_service;

    // gọi hàm của service
    let r = iloveimg_upscale_img_service
        .lock()
        .await
        .upscale_images(files)
        .await
        .map_err(|e| e.to_string());

    r
}
