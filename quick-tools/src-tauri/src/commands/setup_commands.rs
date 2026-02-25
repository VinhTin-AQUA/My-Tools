use tauri::{AppHandle, Manager};
use tokio::time::{sleep, Duration};

#[tauri::command]
pub async fn init_complete(app: AppHandle) -> Result<bool, ()> {
    sleep(Duration::from_secs(3)).await;

    #[cfg(not(target_os = "android"))]
    {
        
    }

    #[cfg(target_os = "android")]
    {
        // app.emit_all("splash-complete", ()).unwrap();
    }

    Ok(true)
}
