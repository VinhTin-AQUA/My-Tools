use serde::Serialize;
use tauri::command;

#[derive(Serialize)]
pub struct FileMetadata {
    pub name: String,
    pub size: u64,
}

#[command]
pub async fn get_file_metadata(
    path: String,
) -> Result<FileMetadata, String> {
    #[cfg(target_os = "android")]
    {
        get_android_metadata(path).await
    }

    #[cfg(not(target_os = "android"))]
    {
        get_desktop_metadata(path).await
    }
}

#[cfg(not(target_os = "android"))]
async fn get_desktop_metadata(
    path: String,
) -> Result<FileMetadata, String> {
    use tokio::fs;

    let metadata = fs::metadata(&path)
        .await
        .map_err(|e| e.to_string())?;

    let name = std::path::Path::new(&path)
        .file_name()
        .ok_or("Invalid filename")?
        .to_string_lossy()
        .to_string();

    Ok(FileMetadata {
        name,
        size: metadata.len(),
    })
}

#[cfg(target_os = "android")]
async fn get_android_metadata(
    path: String,
) -> Result<FileMetadata, String> {
    // temporary fallback
    // later you can replace by ContentResolver query

    let filename =
        format!("image_{}.png", uuid::Uuid::new_v4());

    Ok(FileMetadata {
        name: filename,
        size: 0,
    })
}
