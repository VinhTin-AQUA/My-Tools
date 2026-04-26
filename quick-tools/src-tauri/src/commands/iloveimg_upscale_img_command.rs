use crate::states::AppState;
use tauri::{command, State};
use tokio::sync::Mutex;

#[command]
pub async fn iloveimg_upscale_img_command(
    state: State<'_, Mutex<AppState>>,
    scale: String,
    files: Vec<String>,
) -> Result<Vec<String>, String> {
    let state_guard = state.lock().await;
    let service = state_guard.iloveimg_upscale_img_service.lock().await;

    let results = service.upscale_images(&scale, files).await?;

    let output_paths = results
        .into_iter()
        .map(|p| p.to_string_lossy().to_string())
        .collect();

    Ok(output_paths)
}
