import { UpscaleImageRequest } from "../upload-image.model";

export interface BinaryFile {
    name: string;
    bytes: number[];
}

export interface ILoveImgUpscalePayloadCommand {
    files: UpscaleImageRequest[];
    scale: string,
}
