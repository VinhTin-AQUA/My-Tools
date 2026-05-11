import { SavedHandle } from "../../../core/models/upload-image.model";

export interface UpscaleResult {
    id: string;

    filename: string;

    src: string;

    handle: SavedHandle;

    file_size: number;

    downloaded: boolean;
}
