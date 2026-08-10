export enum ProcessingStatus {
    PROCESSING,
    FAILED,
    SUCCESS,
}

export interface FileModel {
    id: string;
    name: string;
    size: number;
    base64: string;
    previewUrl: string;
    selected?: boolean;
    processingStatus: ProcessingStatus;
}
