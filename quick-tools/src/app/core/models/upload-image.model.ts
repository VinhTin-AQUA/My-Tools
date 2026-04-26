export interface UploadImage {
    id: string;
    filename: string;
    base64: string;
    file_size: number;
    downloaded: boolean;
}

export interface UpscaleImageRequest {
     id: string,
     path: string,
}


export interface UpscaleImageResult {
    id: string;
    path: string;
}
