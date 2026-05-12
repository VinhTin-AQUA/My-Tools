export interface UpscaleImageFile {
    id: string;
    fileName: string;
    path: string
}

export interface ILoveImgUpscalePayloadCommand {
    files: UpscaleImageFile[];
    scale: string,
}
