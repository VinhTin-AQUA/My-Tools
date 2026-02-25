use aes_gcm::aead::{Aead, KeyInit};
use aes_gcm::{Aes256Gcm, Key, Nonce}; // AES-GCM
use argon2::Argon2;
use base64::{engine::general_purpose, Engine as _};
use rand::RngCore;

/// Mã hóa plaintext với mật khẩu, trả về (salt_base64, nonce+ciphertext_base64)
pub fn encrypt_data(password: &str, plaintext: &str, salt: [u8; 16]) -> String {
    // Sinh key từ password + salt
    let key = derive_key_from_password(password, &salt);
    let cipher = Aes256Gcm::new(&key);

    // Tạo nonce ngẫu nhiên
    let mut nonce_bytes = [0u8; 12];
    rand::thread_rng().fill_bytes(&mut nonce_bytes);
    let nonce = Nonce::from_slice(&nonce_bytes);

    // Mã hóa
    let ciphertext = cipher
        .encrypt(nonce, plaintext.as_bytes())
        .expect("Encryption failed");

    // Trả về Base64
    let data_b64 =
        general_purpose::STANDARD.encode([nonce_bytes.as_ref(), ciphertext.as_ref()].concat());

    data_b64
}

/// Giải mã dữ liệu với password + salt
pub fn decrypt_data(password: &str, salt_b64: &str, data_b64: &str) -> Result<String, String> {
    let salt = general_purpose::STANDARD
        .decode(salt_b64)
        .map_err(|e| e.to_string())?;
    let data = general_purpose::STANDARD
        .decode(data_b64)
        .map_err(|e| e.to_string())?;

    if data.len() < 12 {
        return Err("Data too short".into());
    }

    let (nonce_bytes, ciphertext) = data.split_at(12);
    let nonce = Nonce::from_slice(nonce_bytes);

    let key = derive_key_from_password(password, &salt);
    let cipher = Aes256Gcm::new(&key);

    let plaintext_bytes = cipher
        .decrypt(nonce, ciphertext)
        .map_err(|_| "Decryption failed".to_string())?;
    String::from_utf8(plaintext_bytes).map_err(|e| e.to_string())
}

/// Sinh key AES-256 từ mật khẩu và salt
fn derive_key_from_password(password: &str, salt: &[u8]) -> Key<Aes256Gcm> {
    let argon2 = Argon2::default();
    let mut key_bytes: [u8; 32] = [0u8; 32]; // AES-256 yêu cầu 32 bytes
    argon2
        .hash_password_into(password.as_bytes(), salt, &mut key_bytes)
        .expect("Password hashing failed");

    // Explicitly tạo Key với kiểu Aes256Gcm
    Key::<Aes256Gcm>::from_slice(&key_bytes).clone()
}
