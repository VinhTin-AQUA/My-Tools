use crate::services::{AnswerService, DatabaseService, QuestionService};
use std::sync::Arc;
use tokio::sync::Mutex;

pub struct AppState {
    pub db_service: Arc<Mutex<DatabaseService>>,
    pub question_service: Arc<Mutex<QuestionService>>,
    pub answer_service: Arc<Mutex<AnswerService>>,
}
