use serde::{Deserialize, Serialize};

#[derive(Debug, Serialize, Deserialize)]
pub struct Answer {
    pub id: i64,
    pub content: String,
    pub is_correct: bool,
    pub question_id: i64,
}

#[derive(Debug, Serialize, Deserialize)]
pub struct AddAnswer {
    pub content: String,
    pub is_correct: bool,
    pub question_id: i64,
}
