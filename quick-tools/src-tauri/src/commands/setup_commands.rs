use tauri::AppHandle;


#[tauri::command]
pub async fn init_complete() -> Result<bool, ()> {
    #[cfg(not(target_os = "android"))]
    {}

    #[cfg(target_os = "android")]
    {
        // app.emit_all("splash-complete", ()).unwrap();
    }

    Ok(true)
}
