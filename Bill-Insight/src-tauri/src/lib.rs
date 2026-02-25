use std::sync::Arc;

mod commands;
mod helpers;
mod models;
mod services;
mod states;

use commands::*;
use services::{BachHoaXanhService, GoogleSheetsService};
use states::AppState;
use tauri::Manager;
use tokio::sync::Mutex;

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
    let state = Mutex::new(AppState {
        bhx_service: Mutex::new(BachHoaXanhService::new()),
        google_sheet_service: Mutex::new(GoogleSheetsService::new()),
    });

    tauri::Builder::default()
        .plugin(tauri_plugin_store::Builder::new().build())
        .plugin(tauri_plugin_fs::init())
        .setup(|app| {
            if cfg!(debug_assertions) {
                app.handle().plugin(
                    tauri_plugin_log::Builder::default()
                        .level(log::LevelFilter::Info)
                        .build(),
                )?;
            }

            app.manage(state);
            Ok(())
        })
        .invoke_handler(tauri::generate_handler![
            init_google_sheet_command,
            get_invoices,
            set_invoices,
            get_sheet_stats,
            list_sheets,
            get_captcha_and_asp_session,
            get_xml_invoice_data,
            update_sheet_name
        ])
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}
