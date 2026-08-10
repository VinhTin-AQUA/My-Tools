import { ProcessingStatus } from "../../../models/file.model";

export interface UpscaleImageRequest {
    scale: string;
    upscaleImageRequestItems: UpscaleImageRequestItem[];
}

export interface UpscaleImageRequestItem {
    id: string;
    name: string;
    base64: string;
    localPath: string;
}

export interface UpscaleImageResponseItem {
    id: string;
    name: string;
    processingStatus: ProcessingStatus;
    localPath: string;
    size: number;
}
