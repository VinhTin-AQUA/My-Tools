use crate::{
    constants::{app_folder_constants, database_constants, file_name_constants::EXPORT_FILE_NAME},
    models::{Answer, ExportDataFile, Question},
};
use anyhow::{anyhow, Result};
use dirs;
use std::path::PathBuf;
use tokio::{
    fs::{self, File},
    io::AsyncWriteExt,
};
use turso::{params, Builder, Connection, Row};

pub struct DatabaseService {
    connection: Connection,
}

impl DatabaseService {
    pub async fn new() -> Result<Self> {
        let data_local_dir = dirs::data_local_dir().unwrap_or(PathBuf::from("./db"));
        let db_path = data_local_dir
            .join(app_folder_constants::APP_PACKAGE)
            .join(database_constants::DB_PATH);

        // Pure async, không blocking
        let db = Builder::new_local(db_path.to_string_lossy().to_string().as_str())
            .build()
            .await
            .map_err(|e| anyhow!("Failed to build database: {}", e))?;

        let conn = db
            .connect()
            .map_err(|e| anyhow!("Failed to connect: {}", e))?;

        // Tạo bảng posts
        conn.execute(
            r#"
                    CREATE TABLE IF NOT EXISTS questions (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        content TEXT NOT NULL
                    );
                "#,
            (),
        )
        .await
        .map_err(|e| anyhow!("Failed to create table: {}", e))?;

        // Tạo bảng users
        conn.execute(
            r#"
                    CREATE TABLE IF NOT EXISTS answers (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        content TEXT NOT NULL,
                        is_correct BOOLEAN DEFAULT 0,
                        question_id INTEGER NOT NULL
                    );
                "#,
            (),
        )
        .await
        .map_err(|e| anyhow!("Failed to create users table: {}", e))?;

        Ok(DatabaseService { connection: conn })
    }

    pub async fn get_connection(&self) -> &Connection {
        &self.connection
    }

    pub async fn get_connection_mut(&mut self) -> &mut Connection {
        &mut self.connection
    }

    pub async fn query(&self, sql: &str, params: Vec<turso::Value>) -> Result<Vec<Row>> {
        let conn = self.get_connection().await;
        let mut rows = conn
            .query(sql, params)
            .await
            .map_err(|e| anyhow!("Query failed: {}", e))?;

        let mut result = Vec::new();
        while let Some(row) = rows
            .next()
            .await
            .map_err(|e| anyhow!("Row iteration failed: {}", e))?
        {
            result.push(row);
        }

        Ok(result)
    }

    // 🔵 Helper method để query single row
    pub async fn query_one(&self, sql: &str, params: Vec<turso::Value>) -> Result<Option<Row>> {
        let mut rows = self.query(sql, params).await?;
        Ok(rows.pop())
    }

    pub async fn expot_json_data(&self) -> Result<bool> {
        let question_query = "SELECT id, content FROM questions";
        let conn = self.get_connection().await;

        let mut question_rows = conn
            .query(question_query, [0])
            .await
            .map_err(|e| anyhow!("Query failed: {}", e))?;

        let mut questions: Vec<Question> = Vec::new();
        while let Some(row) = question_rows.next().await? {
            questions.push(Question {
                id: row.get::<i64>(0).unwrap_or(-1),
                content: row.get::<String>(1).unwrap_or("".to_string()),
            });
        }

        let answer_query = "SELECT id, content, is_correct, question_id FROM answers";
        let mut answer_rows = conn
            .query(answer_query, [0])
            .await
            .map_err(|e| anyhow!("Query failed: {}", e))?;

        let mut answers: Vec<Answer> = Vec::new();
        while let Some(row) = answer_rows.next().await? {
            answers.push(Answer {
                id: row.get::<i64>(0).unwrap_or(-1),
                content: row.get::<String>(1).unwrap_or("".to_string()),
                is_correct: row.get::<bool>(2).unwrap_or(false),
                question_id: row.get::<i64>(3).unwrap_or(-1),
            });
        }

        let export_data_file: ExportDataFile = ExportDataFile { questions, answers };
        let json = serde_json::to_string_pretty(&export_data_file).unwrap();

        let mut download_dir = dirs::download_dir().unwrap_or(PathBuf::from("./"));
        download_dir.push(EXPORT_FILE_NAME);

        let mut file = File::create(&download_dir).await?;
        file.write_all(json.as_bytes()).await?;
        file.flush().await?;

        Ok(true)
    }

    pub async fn import_json_to_db(&mut self, path: &str) -> Result<bool> {
        // TRY đọc file
        let text = match fs::read_to_string(path).await {
            Ok(t) => t,
            Err(e) => {
                eprintln!("❌ Lỗi đọc file: {}", e);
                return Err(e.into());
            }
        };

        // TRY parse JSON
        let data: ExportDataFile = match serde_json::from_str(&text) {
            Ok(d) => d,
            Err(e) => {
                eprintln!("❌ Lỗi parse JSON: {}", e);
                return Err(e.into());
            }
        };

        if data.questions.is_empty() && data.answers.is_empty() {
            return Ok(false);
        }

        let conn = self.get_connection_mut().await;
        let tx = match conn.transaction().await {
            Ok(t) => t,
            Err(e) => {
                eprintln!("❌ Không thể tạo transaction: {}", e);
                return Err(e.into());
            }
        };

        // TRY insert questions
        for question in data.questions {
            if let Err(e) = tx
                .execute(
                    "INSERT INTO questions (id, content) VALUES (?1, ?2)",
                    params![question.id, question.content],
                )
                .await
            {
                eprintln!("❌ Lỗi insert question: {}", e);
                tx.rollback().await.ok();
                return Err(e.into());
            }
        }

        // TRY insert answers
        for ans in data.answers {
            let is_correct = if ans.is_correct { 1 } else { 0 };

            if let Err(e) = tx
                .execute(
                    "INSERT INTO answers (content, is_correct, question_id)
             VALUES (?1, ?2, ?3)",
                    params![ans.content, is_correct, ans.question_id],
                )
                .await
            {
                eprintln!("❌ Lỗi insert answer: {}", e);
                tx.rollback().await.ok();
                return Err(e.into());
            }
        }

        // TRY commit
        if let Err(e) = tx.commit().await {
            eprintln!("❌ Lỗi commit transaction: {}", e);
            return Err(e.into());
        }

        Ok(true)
    }

    pub async fn delete_all_data(&mut self) -> Result<bool> {
        let delete_question_query = "DELETE FROM questions;";
        let delete_answer_query = "DELETE FROM answers;";
        let conn = self.get_connection_mut().await;
        let tx = conn.transaction().await?;

        tx.execute(delete_question_query, params![]).await?;
        tx.execute(delete_answer_query, params![]).await?;

        tx.commit().await?;

        Ok(true)
    }
}
