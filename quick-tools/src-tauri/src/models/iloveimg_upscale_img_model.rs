use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct UploadResponse {
    pub server_filename: String,
}

#[derive(Deserialize, Debug)]
pub struct BinaryFile {
    pub name: String,
    pub bytes: Vec<u8>,
}

#[derive(Serialize, Deserialize)]
pub struct UpscaleImageRequest {
    pub id: String,
    pub path: String,
}

#[derive(Serialize, Deserialize, Clone)]
pub struct UpscaleImageResult {
    pub id: String,
    pub path: String,
}
