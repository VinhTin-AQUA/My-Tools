mod commands;
use std::sync::Arc;
mod services;
use services::*;
mod models;
mod states;
use states::*;
use tauri::Manager;
use tokio::sync::Mutex;
mod bootstrapper;
mod constants;
use bootstrapper::*;
use commands::*;

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
    tauri::Builder::default()
        .plugin(tauri_plugin_dialog::init())
        .plugin(tauri_plugin_fs::init())
        .setup(|app| {
            if cfg!(debug_assertions) {
                app.handle().plugin(
                    tauri_plugin_log::Builder::default()
                        .level(log::LevelFilter::Info)
                        .build(),
                )?;
            }

            init();

            // block_on để chạy async trong setup
            let db_service =
                tauri::async_runtime::block_on(async { DatabaseService::new().await.unwrap() });
            let db_service = Arc::new(Mutex::new(db_service));
            let question_service = Arc::new(Mutex::new(QuestionService::new(db_service.clone())));
            let answer_service = Arc::new(Mutex::new(AnswerService::new(db_service.clone())));

            let state = AppState {
                db_service,
                question_service,
                answer_service,
            };

            app.manage(Arc::new(state));

            Ok(())
        })
        .invoke_handler(tauri::generate_handler![
            add_question,
            add_answers,
            get_all_questions,
            get_answers_by_question_id,
            delete_question_by_id,
            delete_answers_by_question_id,
            delete_answer_by_id,
            update_question_by_id,
            update_answers,
            get_question_by_id,
            get_total_questions,
            get_random_questions,
            expot_json_data,
            import_json_to_db,
            delete_all_data
        ])
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}
