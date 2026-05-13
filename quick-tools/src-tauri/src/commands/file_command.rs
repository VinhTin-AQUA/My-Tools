// use serde::Serialize;
// use tauri::command;

// #[derive(Serialize)]
// pub struct FileMetadata {
//     pub name: String,
//     pub size: u64,
// }

// #[command]
// pub async fn get_file_metadata(
//     path: String,
// ) -> Result<FileMetadata, String> {
//     #[cfg(target_os = "android")]
//     {
//         get_android_metadata(path).await
//     }

//     #[cfg(not(target_os = "android"))]
//     {
//         get_desktop_metadata(path).await
//     }
// }

// #[cfg(not(target_os = "android"))]
// async fn get_desktop_metadata(
//     path: String,
// ) -> Result<FileMetadata, String> {
//     use tokio::fs;

//     let metadata = fs::metadata(&path)
//         .await
//         .map_err(|e| e.to_string())?;

//     let name = std::path::Path::new(&path)
//         .file_name()
//         .ok_or("Invalid filename")?
//         .to_string_lossy()
//         .to_string();

//     Ok(FileMetadata {
//         name,
//         size: metadata.len(),
//     })
// }

// #[cfg(target_os = "android")]
// async fn get_android_metadata(
//     path: String,
// ) -> Result<FileMetadata, String> {
//     // temporary fallback
//     // later you can replace by ContentResolver query

//     let filename =
//         format!("image_{}.png", uuid::Uuid::new_v4());

//     Ok(FileMetadata {
//         name: filename,
//         size: 0,
//     })
// }

use std::{
    fs::{self, File},
    io::{Read, Write},
};

use tauri::Manager;
use tauri_plugin_android_fs::{AndroidFsExt, FileAccessMode, FileUri};

use crate::constants::app_constants;

#[tauri::command]
pub fn copy_file_from_uri(app: tauri::AppHandle, uri: FileUri) -> Result<String, String> {
    let api = app.android_fs();

    let mut input = api
        .open_file(&uri, tauri_plugin_android_fs::FileAccessMode::Read)
        .map_err(|e| e.to_string())?;

    let info = api.get_info(&uri).map_err(|e| e.to_string())?;
    let file_name = info.name();

    let base_dir = app.path().app_data_dir().map_err(|e| e.to_string())?;
    let images_dir = base_dir.join(app_constants::IMAGES);

    fs::create_dir_all(&images_dir).map_err(|e| e.to_string())?;
    let out_path = images_dir.join(file_name);
    let mut output = std::fs::File::create(&out_path).map_err(|e| e.to_string())?;
    let mut buffer = [0u8; 8192];

    loop {
        let size = input.read(&mut buffer).map_err(|e| e.to_string())?;

        if size == 0 {
            break;
        }

        output
            .write_all(&buffer[..size])
            .map_err(|e| e.to_string())?;
    }

    Ok(out_path.to_string_lossy().to_string())
}
