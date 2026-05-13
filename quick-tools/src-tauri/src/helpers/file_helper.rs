use std::path::PathBuf;

use tauri::{AppHandle, Manager};
use tauri_plugin_android_fs::{AndroidFsExt, PublicImageDir};

use crate::constants::app_constants;

// Save bytes into platform public directory
pub async fn save_bytes(app: &AppHandle, file_name: &str, bytes: &[u8]) -> Result<PathBuf, String> {
    #[cfg(target_os = "android")]
    {
        save_android(app, file_name, bytes).await
    }

    #[cfg(not(target_os = "android"))]
    {
        save_desktop(file_name, bytes).await
    }
}

// Save bytes into platform private directory
pub async fn save_bytes_to_private_dir(
    app: &AppHandle,
    folder: &str,
    file_name: &str,
    bytes: &[u8],
) -> Result<PathBuf, String> {
    let app_dir = app.path().app_data_dir().map_err(|e| e.to_string())?;
    let target_dir = app_dir.join(folder);

    if !target_dir.exists() {
        std::fs::create_dir_all(&target_dir).map_err(|e| e.to_string())?;
    }

    let file_path = target_dir.join(file_name);
    std::fs::write(&file_path, bytes).map_err(|e| e.to_string())?;

    Ok(file_path)
}

/*****************************************************************************/

#[cfg(not(target_os = "android"))]
async fn save_desktop(file_name: &str, bytes: &[u8]) -> Result<PathBuf, String> {
    use crate::constants::app_constants;

    let download_dir = dirs::download_dir()
        .ok_or_else(|| "Cannot find Downloads folder".to_string())?
        .join(app_constants::SCALED_FOLDER);

    tokio::fs::create_dir_all(&download_dir)
        .await
        .map_err(|e| e.to_string())?;

    let output_path = download_dir.join(file_name);

    tokio::fs::write(&output_path, bytes)
        .await
        .map_err(|e| e.to_string())?;

    println!("Saved: {:?}", output_path);

    Ok(output_path)
}

#[cfg(target_os = "android")]
async fn save_android(app: &AppHandle, file_name: &str, bytes: &[u8]) -> Result<PathBuf, String> {
    let api = app.android_fs_async();

    let granted = api
        .public_storage()
        .request_permission()
        .await
        .map_err(|e| e.to_string())?;

    if !granted {
        return Err("Storage permission denied".to_string());
    }

    api.public_storage()
        .write_new(
            None,
            PublicImageDir::Pictures,
            format!("{}/{}", app_constants::APP_NAME, file_name),
            Some("image/png"),
            bytes,
        )
        .await
        .map_err(|e| e.to_string())?;

    Ok(PathBuf::new())
}
