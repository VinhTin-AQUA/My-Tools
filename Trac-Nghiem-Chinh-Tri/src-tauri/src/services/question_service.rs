use std::sync::Arc;

use crate::{
    models::{AddQuestion, Question},
    services::DatabaseService,
};
use anyhow::{anyhow, Result};
use tokio::sync::Mutex;

pub struct QuestionService {
    db: Arc<Mutex<DatabaseService>>,
}

impl QuestionService {
    pub fn new(db: Arc<Mutex<DatabaseService>>) -> Self {
        Self { db }
    }

    pub async fn add_question(&self, new_question: AddQuestion) -> Result<i64> {
        let db = self.db.lock().await;
        let conn = db.get_connection().await;

        let t = conn
            .execute(
                "INSERT INTO questions (content) VALUES (?1)",
                [new_question.content],
            )
            .await?;

        let last_insert_rowid = conn.last_insert_rowid();

        Ok(last_insert_rowid)
    }

    pub async fn get_all_questions(&self) -> Result<Vec<Question>> {
        let sql = "SELECT id, content FROM questions";
        let db = self.db.lock().await;
        let conn = db.get_connection().await;

        let mut rows = conn
            .query(sql, [0])
            .await
            .map_err(|e| anyhow!("Query failed: {}", e))?;

        let mut questions: Vec<Question> = Vec::new();

        // doc ket qua
        while let Some(row) = rows.next().await? {
            questions.push(Question {
                id: row.get::<i64>(0).unwrap_or(0),
                content: row.get::<String>(1).unwrap_or("".to_string()),
            });
        }

        Ok(questions)
    }

    pub async fn get_question_by_id(&self, question_id: i64) -> Result<Question> {
        let sql = "SELECT id, content FROM questions where id = (?1);";
        let db = self.db.lock().await;
        let conn = db.get_connection().await;

        let mut rows = conn
            .query(sql, [question_id])
            .await
            .map_err(|e| anyhow!("Query failed: {}", e))?;

        let mut questions: Vec<Question> = Vec::new();

        // doc ket qua
        while let Some(row) = rows.next().await? {
            questions.push(Question {
                id: row.get::<i64>(0).unwrap_or(0),
                content: row.get::<String>(1).unwrap_or("".to_string()),
            });
            break;
        }

        // Lấy phần tử đầu tiên
        questions
            .into_iter()
            .next()
            .ok_or_else(|| anyhow::anyhow!("Không tìm thấy câu hỏi với id {}", question_id))
    }

    pub async fn delete_question_by_id(&self, question_id: i64) -> Result<bool> {
        let db = self.db.lock().await;
        let conn = db.get_connection().await;

        conn.execute("DELETE FROM questions WHERE id = (?1);", [question_id])
            .await?;

        Ok(true)
    }

    pub async fn update_question_by_id(&self, question: Question) -> Result<bool> {
        let db = self.db.lock().await;
        let conn = db.get_connection().await;

        conn.execute(
            "
                UPDATE questions
                SET content = (?1)
                WHERE id = (?2);
            ",
            [question.content, question.id.to_string()],
        )
        .await?;

        Ok(true)
    }

    pub async fn get_total_questions(&self) -> Result<i64> {
        let sql = "SELECT COUNT(*) FROM  questions;";
        let db = self.db.lock().await;
        let conn = db.get_connection().await;

        // Thực hiện query và lấy số lượng bản ghi
        let mut rows = conn
            .query(sql, [0])
            .await
            .map_err(|e| anyhow!("Query failed: {}", e))?;

        let mut count: i64 = 0;

        // doc ket qua
        while let Some(row) = rows.next().await? {
            count = row.get::<i64>(0).unwrap_or(0);
            break;
        }

        Ok(count)
    }

    pub async fn get_random_questions(&self, rand: i64) -> Result<Vec<Question>> {
        let sql = "SELECT id, content FROM questions ORDER BY RANDOM() LIMIT (?1)";
        let db = self.db.lock().await;
        let conn = db.get_connection().await;

        let mut rows = conn
            .query(sql, [rand])
            .await
            .map_err(|e| anyhow!("Query failed: {}", e))?;

        let mut questions: Vec<Question> = Vec::new();

        // doc ket qua
        while let Some(row) = rows.next().await? {
            questions.push(Question {
                id: row.get::<i64>(0).unwrap_or(0),
                content: row.get::<String>(1).unwrap_or("".to_string()),
            });
        }

        Ok(questions)
    }
}
