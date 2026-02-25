use serde::{Deserialize, Serialize};

#[derive(Debug, Serialize, Deserialize)]
pub struct Password {
    pub id: String,
    pub account_name: String,
    pub user_name: String,
    pub password: String,
    pub note: String,
    pub salt_base64: String,
}
