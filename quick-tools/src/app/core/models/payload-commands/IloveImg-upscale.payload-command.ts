export interface BinaryFile {
    name: string;
    bytes: number[];
}

export interface ILoveImgUpscalePayloadCommand {
    files: BinaryFile[];
}
