use crate::{
    constants::app_constants,
    fs_kit::{FsResult, SavedHandle, UserFileStore},
};
use async_trait::async_trait;
use mime_guess::from_path;
use tauri::AppHandle;
use tauri_plugin_android_fs::{AndroidFsExt, PublicImageDir};
use tokio::fs;

pub struct AndroidFileStore {
    app: AppHandle,
}

impl AndroidFileStore {
    pub fn new(app: AppHandle) -> Self {
        Self { app }
    }
}

#[async_trait]
impl UserFileStore for AndroidFileStore {
    async fn save(&self, bytes: &[u8], suggested_name: &str) -> FsResult<SavedHandle> {
        let api = self.app.android_fs_async();

        if !api.public_storage().request_permission().await? {
            return Err("Uri not supported on desktop".into());
        }

        let mime = from_path(suggested_name).first_or_octet_stream();
        let mime_str: &str = mime.as_ref();
        let uri = api
            .public_storage()
            .write_new(
                None,
                PublicImageDir::Pictures,
                format!("{} {}", app_constants::APP_NAME, suggested_name),
                Some(mime_str),
                bytes,
            )
            .await?;
        let file_path: tauri_plugin_fs::FilePath = uri.clone().into();
        let path_str: Option<String> = file_path.as_path().map(|p| p.to_string_lossy().to_string());
        let path_str = path_str.ok_or("File path is missing")?.to_string();

        Ok(SavedHandle::Uri(path_str))
    }

    async fn read(&self, handle: &SavedHandle) -> FsResult<Vec<u8>> {
        let api = self.app.android_fs_async();

        if !api.public_storage().request_permission().await? {
            return Err("Uri not supported on desktop".into());
        }

        match handle {
            SavedHandle::Path(_) => Err("Path not supported on desktop".into()),

            SavedHandle::Uri(path) => {
                let bytes = fs::read(path).await?;

                Ok(bytes)
            }
        }
    }
}
