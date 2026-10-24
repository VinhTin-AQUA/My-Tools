export interface PasswordManagerModel {
    id: number,
    accountName: string,
    userName: string,
    password: string,
    note: string,
}

export interface GoogleSheetConfig {
  key: string;
  url: string;
}
