use serde::{Deserialize, Serialize};

#[derive(Debug, Serialize, Deserialize)]
pub struct Question {
    pub id: i64,
    pub content: String,
}

#[derive(Debug, Serialize, Deserialize)]
pub struct AddQuestion {
    pub content: String,
}
