use base64::prelude::*;
use futures::{stream, StreamExt};
use rand::seq::SliceRandom;
use regex::Regex;
use reqwest::header::{HeaderMap, HeaderValue, AUTHORIZATION, ORIGIN, USER_AGENT};
use std::sync::Arc;
use tokio::time::{sleep, Duration};

use crate::models::iloveimg_upscale_img_model::{BinaryFile, UploadResponse, UpscaleResult};

pub struct IloveimgUpscaleImgService {}

static TOKEN: &str = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiIiLCJhdWQiOiIiLCJpYXQiOjE1MjMzNjQ4MjQsIm5iZiI6MTUyMzM2NDgyNCwianRpIjoicHJvamVjdF9wdWJsaWNfYzkwNWRkMWMwMWU5ZmQ3NzY5ODNjYTQwZDBhOWQyZjNfT1Vzd2EwODA0MGI4ZDJjN2NhM2NjZGE2MGQ2MTBhMmRkY2U3NyJ9.qvHSXgCJgqpC4gd6-paUlDLFmg0o2DsOvb1EUYPYx_E";
static USER_AGENT_STR: &str = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36 Edg/131.0.0.0";
static SERVERS: &[&str] = &[
    "api1g", "api2g", "api3g", "api7g", "api8g", "api9g", "api10g", "api11g", "api12g", "api13g",
    "api14g", "api15g", "api16g", "api17g", "api18g", "api19g", "api20g", "api1g", "api1g",
    "api1g", "api2g", "api2g", "api2g", "api3g", "api3g", "api3g", "api11g", "api11g", "api11g",
];

impl IloveimgUpscaleImgService {
    pub fn new() -> Self {
        Self {}
    }

    pub async fn upscale_images(
        &self,
        scale: &str,
        files: Vec<BinaryFile>,
    ) -> Result<Vec<UpscaleResult>, String> {
        // let scale = "1";
        let mut final_results: Vec<UpscaleResult> = vec![];

        // 👉 chia batch 3 file
        for chunk in files.chunks(3) {
            // 🔥 mỗi batch tạo mới server + task_id
            let server = Self::random_server();
            let task_id = Self::get_task_id().await?;

            // upload batch
            let items = Self::upload_images(server.as_str(), task_id.as_str(), chunk).await?;

            let url = format!("https://{}.iloveimg.com/v1/upscale", server);
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

            // ⚡ xử lý tối đa 3 file trong batch (thực ra chunk đã <=3 rồi)
            let results: Vec<Result<UpscaleResult, String>> = stream::iter(items)
                .map(|it| {
                    let client = client.clone();
                    let headers = headers.clone();
                    let url = url.clone();
                    let task_id = task_id.clone();
                    let scale = scale.to_string();

                    async move {
                        let form = reqwest::multipart::Form::new()
                            .text("task", task_id)
                            .text("server_filename", it.server_filename.clone())
                            .text("scale", scale);

                        let bytes = client
                            .post(&url)
                            .headers(headers)
                            .multipart(form)
                            .send()
                            .await
                            .map_err(|e| e.to_string())?
                            .bytes()
                            .await
                            .map_err(|e| e.to_string())?;

                        // let base64_str = BASE64_STANDARD.encode(&bytes);
                        let file_size = bytes.len();
                        let base64_str =
                            tokio::task::spawn_blocking(move || BASE64_STANDARD.encode(&bytes))
                                .await
                                .unwrap();

                        Ok(UpscaleResult {
                            filename: it.server_filename,
                            base64: base64_str,
                            file_size: file_size,
                        })
                    }
                })
                .buffer_unordered(3)
                .collect()
                .await;

            for res in results {
                final_results.push(res?);
            }
            sleep(Duration::from_millis(1000)).await;
        }

        Ok(final_results)
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
        // paths: &[String],
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
