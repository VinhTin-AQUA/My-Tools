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

    #[cfg(target_os = "android")]
    let store = crate::fs_kit::android::AndroidFileStore::new(app_handler.clone());

    #[cfg(not(target_os = "android"))]
    let store = crate::fs_kit::desktop::DesktopFileStore::new();

    let results = service
        .upscale_images(&scale, files, app_handler, &store)
        .await?;

    Ok(results)
}
