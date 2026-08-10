export interface Notification<T> {
    action: string;
    isSuccess: boolean;
    title: string;
    message: string;
    data: T;
}
