export interface UploadImage {
    id: string;
    filename: string;
    base64: string;
    file_size: number;
    downloaded: boolean;
}

export interface UpscaleImageRequest {
    id: string;
    filename: string;
}

export interface UpscaleImageResult {
    id: string;
    path: string
}

export interface FileMetadata {
    name: string;
    size: number;
}
