use async_trait::async_trait;
use serde::{Deserialize, Serialize};

pub type FsResult<T> = Result<T, Box<dyn std::error::Error + Send + Sync>>;

#[derive(Debug, Clone, Serialize, Deserialize)]
#[serde(tag = "kind", content = "value")]
pub enum SavedHandle {
    Path(String),
    Uri(String),
}

#[async_trait]
pub trait UserFileStore: Send + Sync {
    async fn save(&self, bytes: &[u8], suggested_name: &str) -> FsResult<SavedHandle>;

    async fn read(&self, handle: &SavedHandle) -> FsResult<Vec<u8>>;
}

#[cfg(not(target_os = "android"))]
pub mod desktop;

#[cfg(target_os = "android")]
pub mod android;
