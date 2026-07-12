use crate::{
    app_state::AppState,
    models::{AddQuestion, Question},
};
use std::sync::Arc;
use tauri::{command, State};

#[command]
pub async fn add_question(
    state: State<'_, Arc<AppState>>,
    new_question: AddQuestion,
) -> Result<i64, String> {
    let question_service = &state.question_service;

    let r = question_service
        .lock()
        .await
        .add_question(new_question)
        .await
        .map_err(|e| e.to_string());

    r
}

#[command]
pub async fn get_all_questions(state: State<'_, Arc<AppState>>) -> Result<Vec<Question>, String> {
    let question_service = &state.question_service;

    let r = question_service
        .lock()
        .await
        .get_all_questions()
        .await
        .map_err(|e| e.to_string());

    r
}

#[command]
pub async fn delete_question_by_id(
    state: State<'_, Arc<AppState>>,
    question_id: i64,
) -> Result<bool, String> {
    let question_service = &state.question_service;

    let r = question_service
        .lock()
        .await
        .delete_question_by_id(question_id)
        .await
        .map_err(|e| e.to_string());

    r
}

#[command]
pub async fn update_question_by_id(
    state: State<'_, Arc<AppState>>,
    question: Question,
) -> Result<bool, String> {
    let question_service = &state.question_service;

    let r = question_service
        .lock()
        .await
        .update_question_by_id(question)
        .await
        .map_err(|e| e.to_string());

    r
}

#[command]
pub async fn get_question_by_id(
    state: State<'_, Arc<AppState>>,
    question_id: i64,
) -> Result<Question, String> {
    let question_service = &state.question_service;

    let r = question_service
        .lock()
        .await
        .get_question_by_id(question_id)
        .await
        .map_err(|e| e.to_string());

    r
}

#[command]
pub async fn get_total_questions(state: State<'_, Arc<AppState>>) -> Result<i64, String> {
    let question_service = &state.question_service;

    let r = question_service
        .lock()
        .await
        .get_total_questions()
        .await
        .map_err(|e| e.to_string());

    r
}

#[command]
pub async fn get_random_questions(
    state: State<'_, Arc<AppState>>,
    rand: i64,
) -> Result<Vec<Question>, String> {
    let question_service = &state.question_service;

    let r = question_service
        .lock()
        .await
        .get_random_questions(rand)
        .await
        .map_err(|e| e.to_string());

    r
}
