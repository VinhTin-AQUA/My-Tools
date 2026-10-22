export interface PasswordManagerModel {
    id: number,
    accountName: string,
    userName: string,
    password: string,
    note: string,
}

export interface AddPasswordManagerModel {
    accountName: string,
    userName: string,
    password: string,
    note: string,
}

