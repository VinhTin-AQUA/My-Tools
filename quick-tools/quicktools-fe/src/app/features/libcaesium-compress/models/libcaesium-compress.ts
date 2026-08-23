import { ProcessingStatus } from "../../../models/file.model";

export interface LibcaesiumCompressImageRequest {
    quality: number;
    compressImageRequestItems: LibcaesiumCompressImageRequestItem[];
}

export interface LibcaesiumCompressImageRequestItem {
    id: string;
    name: string;
    base64: string;
    localPath: string;
}

export interface LibcaesiumCompressImageResponseItem {
    id: string;
    name: string;
    processingStatus: ProcessingStatus;
    localPath: string;
    size: number;
}
