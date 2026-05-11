use async_trait::async_trait;
use tauri::AppHandle;

use crate::fs_kit::{FsResult, SavedHandle, UserFileStore};

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
        todo!("write SAF save logic")
    }

    async fn read(&self, handle: &SavedHandle) -> FsResult<Vec<u8>> {
        todo!("write SAF read logic")
    }
}
