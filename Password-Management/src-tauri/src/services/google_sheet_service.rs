use crate::helpers::{decrypt_data, encrypt_data};
use crate::models::{Password, ResponseCommand};
use anyhow::anyhow;
use base64::{engine::general_purpose, Engine as _};
use jsonwebtoken::{encode, Algorithm, EncodingKey, Header};
use rand::RngCore;
use reqwest::Client;
use serde::{Deserialize, Serialize};
use serde_json::{json, Value};
use std::fs;
use std::time::{SystemTime, UNIX_EPOCH};

pub struct GoogleSheetsService {
    pub client: Client,
    pub access_token: String,
}

#[derive(Deserialize, Debug)]
struct ServiceAccount {
    client_email: String,
    private_key: String,
}

#[derive(Serialize, Deserialize, Debug)]
pub struct TokenResponse {
    access_token: String,
    expires_in: i64,
    token_type: String,
}

#[derive(Serialize)]
struct Claims<'a> {
    iss: &'a str,
    scope: &'a str,
    aud: &'a str,
    exp: usize,
    iat: usize,
}

impl GoogleSheetsService {
    const BASE_API: &'static str = "https://sheets.googleapis.com/v4/spreadsheets";
    const SCOPE: &'static str = "https://www.googleapis.com/auth/spreadsheets";
    const AUD_URL: &'static str = "https://oauth2.googleapis.com/token";

    pub fn new() -> Self {
        Self {
            client: Client::new(),
            access_token: String::new(),
        }
    }

    pub async fn init_google_service(
        &mut self,
        json_path: &str,
    ) -> anyhow::Result<Option<TokenResponse>> {
        let jwt = Self::get_jwt(json_path)?;
        let token = Self::get_token_response(&jwt).await?;
        let token = if let Some(token_value) = token {
            self.access_token = token_value.access_token.clone();
            Some(token_value)
        } else {
            return Err(anyhow!("Vui lòng đợi và thử lại trong vài giây"));
        };

        Ok(token)
    }

    pub async fn add_account(
        &mut self,
        sheet_name: String,
        spreadsheet_id: String,
        passcode: String,
        password: Password,
    ) -> anyhow::Result<Option<ResponseCommand>> {
        let write_range = format!("{}!A:F", urlencoding::encode(sheet_name.as_str()));
        let write_url = format!(
            "{}/{}/values/{}:append?valueInputOption=RAW&insertDataOption=INSERT_ROWS",
            Self::BASE_API,
            spreadsheet_id,
            write_range
        );

        // Tạo salt ngẫu nhiên
        let mut salt = [0u8; 16];
        rand::thread_rng().fill_bytes(&mut salt);
        let salt_b64 = general_purpose::STANDARD.encode(&salt);

        let row: Vec<Vec<Value>> = vec![vec![
            Value::from(password.id),
            Value::from(password.account_name),
            Value::from(encrypt_data(
                passcode.as_str(),
                password.user_name.as_str(),
                salt,
            )),
            Value::from(encrypt_data(
                passcode.as_str(),
                password.password.as_str(),
                salt,
            )),
            Value::from(encrypt_data(
                passcode.as_str(),
                password.note.as_str(),
                salt,
            )),
            Value::from(salt_b64),
        ]];

        let body = json!({
        "majorDimension": "ROWS",
            "values": row
        });

        _ = self
            .client
            .post(&write_url)
            .bearer_auth(self.access_token.as_str())
            .json(&body)
            .send()
            .await?
            .json::<Value>()
            .await?;

        let response_command: ResponseCommand = ResponseCommand {
            message: "Ghi dữ liệu thành công!".to_string(),
            title: "Success".to_string(),
            is_success: true,
        };
        Ok(Some(response_command))
    }

    pub async fn get_accounts(
        &mut self,
        sheet_name: String,
        spreadsheet_id: String,
        passcode: String,
    ) -> anyhow::Result<Option<Vec<Password>>> {
        let range = format!("{}!A:F", urlencoding::encode(sheet_name.as_str()));
        let read_url = format!("{}/{}/values/{}", Self::BASE_API, spreadsheet_id, range);

        let res = self
            .client
            .get(read_url)
            .bearer_auth(self.access_token.as_str())
            .send()
            .await?
            .json::<Value>()
            .await?;

        let grouped = Self::convert_to_list(&res, passcode);
        Ok(Some(grouped))
    }

    pub async fn delete_account(
        &mut self,
        sheet_name: String,
        spreadsheet_id: String,
        id: String,
    ) -> anyhow::Result<Option<bool>> {
        let range = format!("{}!A:F", urlencoding::encode(sheet_name.as_str()));
        let read_url = format!("{}/{}/values/{}", Self::BASE_API, spreadsheet_id, range);

        let res = self
            .client
            .get(read_url)
            .bearer_auth(self.access_token.as_str())
            .send()
            .await?
            .json::<Value>()
            .await?;

        let Some(values) = res.get("values").and_then(|v| v.as_array()) else {
            return Ok(Some(true));
        };

        // --- 2. Tìm các hàng cần xóa ---
        let mut rows_to_delete = vec![];
        for (i, row) in values.iter().enumerate() {
            if row.get(0).map_or(false, |v| v == id.as_str()) {
                // ví dụ cột B = "abc"
                rows_to_delete.push(i);
            }
        }

        // Quan trọng: xóa từ cuối lên đầu
        rows_to_delete.reverse();

        // --- 3. Gửi batchUpdate để xóa ---
        let requests: Vec<_> = rows_to_delete
            .iter()
            .map(|&row_index| {
                json!({
                    "deleteDimension": {
                        "range": {
                            "sheetId": 0,
                            "dimension": "ROWS",
                            "startIndex": row_index,
                            "endIndex": row_index + 1
                        }
                    }
                })
            })
            .collect();

        let body = json!({ "requests": requests });
        let batch_url: String = format!("{}/{}:batchUpdate", Self::BASE_API, spreadsheet_id);

        _ = self
            .client
            .post(&batch_url)
            .bearer_auth(self.access_token.as_str())
            .json(&body)
            .send()
            .await?;

        Ok(Some(true))
    }

    pub async fn update_account(
        &mut self,
        sheet_name: String,
        spreadsheet_id: String,
        passcode: String,
        password: Password,
    ) -> anyhow::Result<Option<ResponseCommand>> {
        let range = format!("{}!A:F", urlencoding::encode(sheet_name.as_str()));
        let read_url = format!("{}/{}/values/{}", Self::BASE_API, spreadsheet_id, range);

        let res = self
            .client
            .get(read_url)
            .bearer_auth(self.access_token.as_str())
            .send()
            .await?
            .json::<Value>()
            .await?;

        let Some(values) = res.get("values").and_then(|v| v.as_array()) else {
            return Err(anyhow::anyhow!("Missing or invalid `values` field"));
        };

        // --- 2. Tìm các hàng cần cập nhật ---
        let mut rows_to_update = vec![];
        for (i, row) in values.iter().enumerate() {
            // row.get(0) là so sánh cột A
            if row.get(0).map_or(false, |v| v == password.id.as_str()) {
                rows_to_update.push(i);
            }
        }

        if rows_to_update.len() == 0 {
            return Err(anyhow::anyhow!("không tìm thấy id {:?}", password.id));
        }

        rows_to_update.reverse();
        let range = format!(
            "{}!A{}:F{}",
            urlencoding::encode(sheet_name.as_str()),
            rows_to_update[0] + 1,
            rows_to_update[0] + 1
        );

        let update_url = format!(
            "{}/{}/values/{}?valueInputOption=USER_ENTERED",
            Self::BASE_API,
            spreadsheet_id,
            range
        );

        let mut salt = [0u8; 16];
        rand::thread_rng().fill_bytes(&mut salt);
        let salt_b64 = general_purpose::STANDARD.encode(&salt);

        let update_body = json!({
            "range": range,
            "majorDimension": "ROWS",
            "values": [
                [
                    password.id.as_str(),
                    password.account_name.as_str(),
                    encrypt_data(passcode.as_str(), password.user_name.as_str(), salt),
                    encrypt_data(passcode.as_str(), password.password.as_str(),salt),
                    encrypt_data(passcode.as_str(), password.note.as_str(),salt),
                    salt_b64
                ]
            ]
        });

        _ = self
            .client
            .put(&update_url)
            .bearer_auth(self.access_token.as_str())
            .json(&update_body)
            .send()
            .await?;

        let response_command: ResponseCommand = ResponseCommand {
            message: "Ghi dữ liệu thành công!".to_string(),
            title: "Success".to_string(),
            is_success: true,
        };
        Ok(Some(response_command))
    }

    /* private methods */

    fn get_jwt(json_path: &str) -> anyhow::Result<String> {
        let data = fs::read_to_string(json_path)?; // file json được tải ở bước trước trong google console
        let json: ServiceAccount = serde_json::from_str(&data)?;
        let now = SystemTime::now().duration_since(UNIX_EPOCH)?.as_secs();
        let exp = now + 3600; // 1h

        let claims = Claims {
            iss: &json.client_email,
            scope: Self::SCOPE,
            aud: Self::AUD_URL,
            exp: exp as usize,
            iat: now as usize,
        };

        let key = EncodingKey::from_rsa_pem(json.private_key.as_bytes())?;
        let jwt = encode(&Header::new(Algorithm::RS256), &claims, &key)?;
        Ok(jwt)
    }

    async fn get_token_response(jwt: &str) -> anyhow::Result<Option<TokenResponse>> {
        let client = Client::new();
        let params = [
            ("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"),
            ("assertion", jwt),
        ];

        let res = client.post(Self::AUD_URL).form(&params).send().await?;

        if !res.status().is_success() {
            let status = res.status();
            let text = res
                .text()
                .await
                .unwrap_or_else(|_| "<cannot read body>".to_string());

            eprintln!("Error status: {}", status);
            eprintln!("Error body: {}", text);

            return Ok(None);
        }

        let token: TokenResponse = res.json().await?;
        Ok(Some(token))
    }

    fn convert_to_list(value: &Value, passcode: String) -> Vec<Password> {
        // Lấy mảng "values" trong JSON
        let Some(values_array) = value.get("values").and_then(|v| v.as_array()) else {
            return vec![];
        };
        let mut map: Vec<Password> = Vec::new();

        // Bỏ qua dòng tiêu đề
        for row_value in values_array.iter().skip(2) {
            let Some(row) = row_value.as_array() else {
                continue;
            };

            if row.len() < 6 {
                continue;
            }

            let salt_base64 = row[5].as_str().unwrap().trim();

            let id = row[0].as_str().unwrap().trim().to_string();
            let account_name = row[1].as_str().unwrap().trim().to_string();
            let user_name = decrypt_data(
                passcode.as_str(),
                salt_base64,
                row[2].as_str().unwrap().trim(),
            );
            let password = decrypt_data(
                passcode.as_str(),
                salt_base64,
                row[3].as_str().unwrap().trim(),
            );
            let note = decrypt_data(
                passcode.as_str(),
                salt_base64,
                row[4].as_str().unwrap().trim(),
            );

            let item = Password {
                id,
                account_name,
                user_name: user_name.unwrap_or("Failed to decrypt".to_string()),
                password: password.unwrap_or("Failed to decrypt".to_string()),
                note: note.unwrap_or("Failed to decrypt".to_string()),
                salt_base64: salt_base64.to_string(),
            };

            map.push(item);
        }

        map
    }
}
