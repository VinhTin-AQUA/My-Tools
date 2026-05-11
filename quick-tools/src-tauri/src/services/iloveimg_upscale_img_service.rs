use rand::seq::SliceRandom;
use regex::Regex;
use reqwest::header::{HeaderMap, HeaderValue, AUTHORIZATION, ORIGIN, USER_AGENT};
use tauri::{AppHandle, Emitter};
use tokio::time::{sleep, Duration};

use crate::{
    constants::emit_events,
    fs_kit::{SavedHandle, UserFileStore},
    helpers::folder_helper,
    models::iloveimg_upscale_img_model::{
        BinaryFile, UploadResponse, UpscaleImageRequest, UpscaleImageResult,
    },
};

pub struct IloveimgUpscaleImgService {}

static TOKEN: &str = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiIiLCJhdWQiOiIiLCJpYXQiOjE1MjMzNjQ4MjQsIm5iZiI6MTUyMzM2NDgyNCwianRpIjoicHJvamVjdF9wdWJsaWNfYzkwNWRkMWMwMWU5ZmQ3NzY5ODNjYTQwZDBhOWQyZjNfT1Vzd2EwODA0MGI4ZDJjN2NhM2NjZGE2MGQ2MTBhMmRkY2U3NyJ9.qvHSXgCJgqpC4gd6-paUlDLFmg0o2DsOvb1EUYPYx_E";
static USER_AGENT_STR: &str = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36 Edg/131.0.0.0";
static SERVERS: &[&str] = &[
    "api1g", "api2g", "api3g", "api8g", "api9g", "api10g", "api11g", "api12g", "api13g", "api14g",
    "api15g", "api16g", "api17g", "api18g", "api19g", "api20g", "api1g", "api1g", "api1g", "api2g",
    "api2g", "api2g", "api3g", "api3g", "api3g", "api11g", "api11g", "api11g",
]; // "api7g",

impl IloveimgUpscaleImgService {
    pub fn new() -> Self {
        Self {}
    }

    pub async fn upscale_images(
        &self,
        scale: &str,
        upscale_images: Vec<UpscaleImageRequest>,
        app_handler: AppHandle,
        store: &dyn UserFileStore,
    ) -> Result<Vec<UpscaleImageResult>, String> {
        let client = reqwest::Client::new();

        let mut outputs: Vec<UpscaleImageResult> = Vec::new();

        for upscale_image in upscale_images {
            println!("Processing: {}", upscale_image.filename);

            let server = Self::random_server();

            let task_id = Self::get_task_id().await?;

            // IMPORTANT:
            // no tokio::fs::read here
            let bytes = store
                .read(&upscale_image.handle)
                .await
                .map_err(|e| e.to_string())?;

            let file = BinaryFile {
                name: upscale_image.filename.clone(),
                bytes,
            };

            let uploaded = Self::upload_images(server.as_str(), task_id.as_str(), &[file]).await?;

            let uploaded_file = uploaded.get(0).ok_or("Upload failed")?;

            let url = format!("https://{}.iloveimg.com/v1/upscale", server);

            let mut headers = HeaderMap::new();

            headers.insert(USER_AGENT, HeaderValue::from_str(USER_AGENT_STR).unwrap());

            headers.insert(
                AUTHORIZATION,
                HeaderValue::from_str(&format!("Bearer {}", TOKEN)).unwrap(),
            );

            headers.insert(
                ORIGIN,
                HeaderValue::from_str("https://www.iloveimg.com").unwrap(),
            );

            let form = reqwest::multipart::Form::new()
                .text("task", task_id)
                .text("server_filename", uploaded_file.server_filename.clone())
                .text("scale", scale.to_string());

            let result_bytes = client
                .post(&url)
                .headers(headers)
                .multipart(form)
                .send()
                .await
                .map_err(|e| e.to_string())?
                .bytes()
                .await
                .map_err(|e| e.to_string())?;

            // IMPORTANT:
            // no tokio::fs::write here
            let saved_handle = store
                .save(
                    &result_bytes,
                    &format!("upscaled_{}", upscale_image.filename),
                )
                .await
                .map_err(|e| e.to_string())?;

            let result_path = match &saved_handle {
                SavedHandle::Path(path) => path.clone(),
                SavedHandle::Uri(uri) => uri.clone(),
            };

            println!("Saved: {:?}", result_path);

            let result = UpscaleImageResult {
                id: upscale_image.id,
                path: result_path,
            };

            app_handler
                .emit(emit_events::UP_SCALE_IMAGE_RESULT, result.clone())
                .unwrap();

            outputs.push(result);

            sleep(Duration::from_millis(800)).await;
        }

        Ok(outputs)
    }

    fn random_server() -> String {
        let mut rng = rand::thread_rng();
        SERVERS.choose(&mut rng).unwrap().to_string()
    }

    async fn get_task_id() -> Result<String, String> {
        let client = reqwest::Client::new();
        let mut headers = HeaderMap::new();
        headers.insert(USER_AGENT, HeaderValue::from_str(USER_AGENT_STR).unwrap());

        let html = client
            .get("https://www.iloveimg.com/upscale-image")
            .headers(headers)
            .send()
            .await
            .map_err(|e| e.to_string())?
            .text()
            .await
            .map_err(|e| e.to_string())?;

        let re = Regex::new(r"ilovepdfConfig\.taskId\s*=\s*'([^']*)'").unwrap();
        Ok(re
            .captures(&html)
            .and_then(|c| c.get(1))
            .map(|m| m.as_str().to_string())
            .ok_or("Missing taskId")?)
    }

    async fn upload_images(
        server: &str,
        task_id: &str,
        files: &[BinaryFile],
    ) -> Result<Vec<UploadResponse>, String> {
        let url = format!("https://{}.iloveimg.com/v1/upload", server);
        let client = reqwest::Client::new();
        let mut headers = HeaderMap::new();
        headers.insert(USER_AGENT, HeaderValue::from_str(USER_AGENT_STR).unwrap());
        headers.insert(
            AUTHORIZATION,
            HeaderValue::from_str(&format!("Bearer {}", TOKEN)).unwrap(),
        );
        headers.insert(
            ORIGIN,
            HeaderValue::from_str("https://www.iloveimg.com").unwrap(),
        );

        let mut result = vec![];

        for file in files {
            let part =
                reqwest::multipart::Part::bytes(file.bytes.clone()).file_name(file.name.clone());

            let form = reqwest::multipart::Form::new()
                .text("task", task_id.to_string())
                .part("file", part)
                .text("name", file.name.to_string())
                .text("chunks", "1")
                .text("preview", "1")
                .text("pdfinfo", "0")
                .text("pdfforms", "0")
                .text("pdfresetforms", "0")
                .text("v", "web.0");

            let response = client
                .post(&url)
                .headers(headers.clone())
                .multipart(form)
                .send()
                .await;

            match response {
                Ok(resp) => {
                    let text = resp.text().await.map_err(|e| e.to_string())?;
                    result.push(
                        serde_json::from_str(&text)
                            .map_err(|e| format!("Invalid JSON {} => {}", e, text))?,
                    );
                }
                Err(e) => {
                    if e.is_connect() {
                        eprintln!("Connection error: {:?}", e);
                    } else if e.is_timeout() {
                        eprintln!("Timeout error: {:?}", e);
                    } else if e.is_request() {
                        eprintln!("Request error: {:?}", e);
                    } else if e.is_status() {
                        eprintln!("HTTP status error: {:?}", e);
                    } else {
                        eprintln!("Other error: {:?}", e);
                    }
                    return Err(format!("Request failed: {:?}", e));
                }
            }
            sleep(Duration::from_millis(500)).await;
        }

        Ok(result)
    }
}

// i guess since API 29 (scoped storage), outside the app-private directory you don't get a Path — only a content:// URI through the
//   Storage Access Framework (SAF). That's why the plugin returns FileUri instead of PathBuf. You can't make it return a path, because there is no path.

// #[derive(Serialize, Deserialize, Clone)]
// #[serde(tag = "kind", content = "value")]
// pub enum SavedHandle {
//       Path(String),  // Windows/Linux: PathBuf.to_string_lossy().into_owned()
//       Uri(String),   // Android: "content://..." from SAF
//   }

// Rule: never Path::new(&handle.value) on a Uri variant. Read/write only goes through the abstraction below.
// for app-private data you don't need the fork at all — app_data_dir() gives you a real PathBuf everywhere, including Android (it points inside /data/data/<pkg>/files, no SAF involved).

// Thin trait, two impls

//   // fs_kit/mod.rs
//   pub trait UserFileStore {
//       async fn save(&self, bytes: &[u8], suggested_name: &str)
//           -> Result<SavedHandle>;
//       async fn read(&self, handle: &SavedHandle) -> Result<Vec<u8>>;
//   }

//   #[cfg(not(target_os = "android"))]
//   mod desktop {
//       // uses tauri-plugin-dialog + tokio::fs
//       // returns SavedHandle::Path(...)
//   }

//   #[cfg(target_os = "android")]
//   mod android {
//       // uses tauri-plugin-android-fs
//       // returns SavedHandle::Uri(...)
//   }
