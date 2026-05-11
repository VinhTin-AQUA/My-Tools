export interface UploadImage {
    id: string;
    filename: string;
    base64: string;
    file_size: number;
    downloaded: boolean;
}

export interface SavedHandle {
    kind: 'Path' | 'Uri';
    value: string;
}

export interface UpscaleImageRequest {
    id: string;
    filename: string;
    handle: SavedHandle;
}

export interface UpscaleImageResult {
    id: string;
    handle: SavedHandle;
}

export interface FileMetadata {
    name: string;
    size: number;
}
