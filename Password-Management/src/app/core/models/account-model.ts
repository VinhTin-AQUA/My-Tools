export interface AccountModel {
    id: string;
    account_name: string;
    user_name: string;
    password: string;
    note: string;
    salt_base64: string;
}
