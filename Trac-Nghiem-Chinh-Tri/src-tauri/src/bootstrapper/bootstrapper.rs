use crate::constants::{app_folder_constants, database_constants};
use std::fs;
use std::path::PathBuf;

pub fn init() {
    init_folder();
}

fn init_folder() {
    let data_local_dir =
        dirs::data_local_dir().unwrap_or(PathBuf::from(app_folder_constants::APP_PACKAGE));
    let app_package = data_local_dir.join(app_folder_constants::APP_PACKAGE);

    let db_folder = app_package.join(database_constants::DB_FOLDER);
    if !db_folder.exists() {
        if let Err(e) = fs::create_dir_all(db_folder) {
            eprintln!("Cannot create folder: {}", e);
        } else {
            // println!("Đã tạo thư mục mới.");
        }
    }
}
