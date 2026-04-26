use crate::{
    models::iloveimg_upscale_img_model::{UpscaleImageRequest, UpscaleImageResult},
    states::AppState,
};
use tauri::{command, AppHandle, State};
use tokio::sync::Mutex;

#[command]
pub async fn iloveimg_upscale_img_command(
    state: State<'_, Mutex<AppState>>,
    app_handler: AppHandle,
    scale: String,
    files: Vec<UpscaleImageRequest>,
) -> Result<Vec<UpscaleImageResult>, String> {
    let state_guard = state.lock().await;
    let service = state_guard.iloveimg_upscale_img_service.lock().await;

    let results = service.upscale_images(&scale, files, app_handler).await?;

    Ok(results)
}
