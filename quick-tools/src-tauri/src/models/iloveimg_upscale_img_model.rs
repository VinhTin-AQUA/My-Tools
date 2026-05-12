use serde::{Deserialize, Serialize};

use crate::fs_kit::SavedHandle;

pub type FsResult<T> = Result<T, Box<dyn std::error::Error + Send + Sync>>;

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct UploadResponse {
    pub server_filename: String,
}

#[derive(Deserialize, Debug)]
pub struct BinaryFile {
    pub name: String,
    pub bytes: Vec<u8>,
}

#[derive(Serialize, Deserialize, Clone, Debug)]
pub struct UpscaleImageRequest {
    pub id: String,

    pub handle: SavedHandle,

    // không derive filename từ uri
    pub filename: String,
}

#[derive(Serialize, Deserialize, Clone)]
pub struct UpscaleImageResult {
    pub id: String,
    pub path: String,
}
