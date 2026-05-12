use crate::{
    models::iloveimg_upscale_img_model::{UpscaleImageRequest, UpscaleImageResult},
    states::AppState,
};
use tauri::{command, State};
use tokio::sync::Mutex;

#[command]
pub async fn iloveimg_upscale_img_command(
    state: State<'_, Mutex<AppState>>,
    scale: String,
    files: Vec<UpscaleImageRequest>,
) -> Result<Vec<UpscaleImageResult>, String> {
    let state_guard = state.lock().await;

    let service = state_guard.iloveimg_upscale_img_service.lock().await;

    service.upscale_images(&scale, files).await
}
