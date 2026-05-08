use std::path::PathBuf;

pub async fn create_folder_in_download(paths: &[&str]) -> Result<PathBuf, String> {
    if cfg!(any(target_os = "windows", target_os = "linux")) {
        let mut download_dir = dirs::download_dir().ok_or("Cannot find Downloads folder")?;

        for p in paths {
            download_dir = download_dir.join(p);
        }

        tokio::fs::create_dir_all(&download_dir)
            .await
            .map_err(|e| e.to_string())?;

        Ok(download_dir)
    } else if cfg!(target_os = "macos") {
        Err("Unsupported operating system: macos".to_string())
    } else if cfg!(target_os = "android") {
        Err("Unsupported operating system: android".to_string())
    } else if cfg!(target_os = "ios") {
        Err("Unsupported operating system: ios".to_string())
    } else {
        Err("Unsupported operating system: unknown".to_string())
    }
}
