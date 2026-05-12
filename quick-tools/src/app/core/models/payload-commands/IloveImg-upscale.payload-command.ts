import { SelectedFile } from "../../../shared/helpers/file.helper";

// export interface BinaryFile {
//     name: string;
//     bytes: number[];
// }

export interface UpscaleImageFile {
    id: string;
    fileName: string;
    path: string
}

export interface ILoveImgUpscalePayloadCommand {
    files: UpscaleImageFile[];
    scale: string,
}
