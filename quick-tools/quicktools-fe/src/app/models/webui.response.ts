export interface WebUIResponse<T> {
    title: string;
    description: string;
    action: string;
    success: boolean;
    data: T | null;
}
