use crate::{
    app_state::AppState,
    models::{AddAnswer, Answer},
};
use std::sync::Arc;
use tauri::{command, State};

#[command]
pub async fn add_answers(
    state: State<'_, Arc<AppState>>,
    new_answers: Vec<AddAnswer>,
) -> Result<bool, String> {
    let answer_service = &state.answer_service;

    let r = answer_service
        .lock()
        .await
        .add_answers(new_answers)
        .await
        .map_err(|e| e.to_string());

    r
}

#[command]
pub async fn get_answers_by_question_id(
    state: State<'_, Arc<AppState>>,
    question_id: i64,
) -> Result<Vec<Answer>, String> {
    let answer_service = &state.answer_service;

    let r = answer_service
        .lock()
        .await
        .get_answers_by_question_id(question_id)
        .await
        .map_err(|e| e.to_string());

    r
}

#[command]
pub async fn delete_answers_by_question_id(
    state: State<'_, Arc<AppState>>,
    question_id: i64,
) -> Result<bool, String> {
    let question_service = &state.answer_service;

    let r = question_service
        .lock()
        .await
        .delete_answers_by_question_id(question_id)
        .await
        .map_err(|e| e.to_string());

    r
}

#[command]
pub async fn delete_answer_by_id(state: State<'_, Arc<AppState>>, id: i64) -> Result<bool, String> {
    let question_service = &state.answer_service;

    let r = question_service
        .lock()
        .await
        .delete_answer_by_id(id)
        .await
        .map_err(|e| e.to_string());

    r
}

#[command]
pub async fn update_answers(
    state: State<'_, Arc<AppState>>,
    answers: Vec<Answer>,
) -> Result<bool, String> {
    let question_service = &state.answer_service;

    let r = question_service
        .lock()
        .await
        .update_answers(answers)
        .await
        .map_err(|e| e.to_string());

    r
}
