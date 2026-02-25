mod commands;
mod states;

use commands::*;
use i_love_img_upscale_img_lib::IloveimgUpscaleImgService;
use ip_lib::IpService;
use states::AppState;
use tauri::Manager;
use tokio::sync::Mutex;

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
    // tạo state
    let state = Mutex::new(AppState {
        iloveimg_upscale_img_service: Mutex::new(IloveimgUpscaleImgService::new()),
        ip_service: Mutex::new(IpService::new()),
    });

    tauri::Builder::default()
        .plugin(tauri_plugin_os::init())
        .plugin(tauri_plugin_fs::init())
        .plugin(tauri_plugin_android_fs::init())
        .setup(|app| {
            if cfg!(debug_assertions) {
                app.handle().plugin(
                    tauri_plugin_log::Builder::default()
                        .level(log::LevelFilter::Info)
                        .build(),
                )?;
            }

            app.manage(state); // khai báo state ở đây
            Ok(())
        })
        .invoke_handler(tauri::generate_handler![
            iloveimg_upscale_img_command,
            init_complete,
            get_your_ip_command,
            get_your_ip_info_command,
        ])
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}
