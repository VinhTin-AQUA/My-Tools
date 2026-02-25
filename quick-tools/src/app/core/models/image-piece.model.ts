export interface ImagePiece {
    dataURL: string;
    row: number;
    col: number;
    width: number;
    height: number;
    x: number;
    y: number;
    type: string;
    downloaded: boolean;
}