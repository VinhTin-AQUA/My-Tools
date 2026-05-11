use async_trait::async_trait;
use tokio::fs;

use crate::fs_kit::{FsResult, SavedHandle, UserFileStore};

pub struct DesktopFileStore;

impl DesktopFileStore {
    pub fn new() -> Self {
        Self
    }
}

#[async_trait]
impl UserFileStore for DesktopFileStore {
    async fn save(&self, bytes: &[u8], suggested_name: &str) -> FsResult<SavedHandle> {
        let download_dir = dirs::download_dir().ok_or("Cannot find download dir")?;

        let output_dir = download_dir.join("upscale");

        fs::create_dir_all(&output_dir).await?;

        let output = output_dir.join(suggested_name);

        fs::write(&output, bytes).await?;

        Ok(SavedHandle::Path(output.to_string_lossy().to_string()))
    }

    async fn read(&self, handle: &SavedHandle) -> FsResult<Vec<u8>> {
        match handle {
            SavedHandle::Path(path) => {
                let bytes = fs::read(path).await?;

                Ok(bytes)
            }

            SavedHandle::Uri(_) => Err("Uri not supported on desktop".into()),
        }
    }
}
