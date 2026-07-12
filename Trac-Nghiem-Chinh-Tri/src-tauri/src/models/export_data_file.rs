use serde::{Deserialize, Serialize};

use crate::models::{Answer, Question};

#[derive(Serialize, Deserialize)]
pub struct ExportDataFile {
    pub questions: Vec<Question>,
    pub answers: Vec<Answer>,
}
