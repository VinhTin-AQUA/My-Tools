use serde::{Deserialize, Serialize};

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ResponseCommand {
    pub title: String,
    pub message: String,
    pub is_success: bool,
}
