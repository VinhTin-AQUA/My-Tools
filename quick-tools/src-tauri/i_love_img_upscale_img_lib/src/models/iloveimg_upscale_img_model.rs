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

#[derive(Serialize)]
pub struct UpscaleResult {
    pub filename: String,
    pub base64: String,
    pub file_size: usize,
}
