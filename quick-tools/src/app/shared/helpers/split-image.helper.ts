import { ImagePiece } from '../../core/models/image-piece.model';

export class SplitImageHelper {
    static async splitImage(
        image: string | HTMLImageElement | HTMLCanvasElement,
        verticalQuantity: number,
        horizontalQuantity: number
    ): Promise<ImagePiece[]> {
        if (verticalQuantity === 0 && horizontalQuantity === 0) {
            throw new Error('verticalQuantity hoặc horizontalQuantity phải > 0');
        }

        const direction =
            verticalQuantity > 0 && horizontalQuantity > 0 ? 2 : verticalQuantity > 0 ? 0 : 1;

        const options = {
            direction,
            vertical: {
                quantity: verticalQuantity,
                overlap: true,
                overlapAmount: 10,
            },
            horizontal: {
                quantity: horizontalQuantity,
                overlap: true,
                overlapAmount: 10,
            },
            format: {
                type: 'image/png',
                quality: 0.92,
            },
        };

        const sourceCanvas = await this.createSourceCanvas(image);
        return this.processCanvas(sourceCanvas, options);
    }

    // ================= HELPERS =================

    private static createSourceCanvas(
        image: string | HTMLImageElement | HTMLCanvasElement
    ): Promise<HTMLCanvasElement> {
        return new Promise((resolve, reject) => {
            if (image instanceof HTMLCanvasElement) {
                return resolve(image);
            }

            const img = image instanceof HTMLImageElement ? image : new Image();
            if (typeof image === 'string') {
                img.crossOrigin = 'anonymous';
                img.src = image;
            }

            img.onload = () => {
                const canvas = document.createElement('canvas');
                canvas.width = img.naturalWidth;
                canvas.height = img.naturalHeight;
                const ctx = canvas.getContext('2d')!;
                ctx.drawImage(img, 0, 0);
                resolve(canvas);
            };

            img.onerror = reject;
        });
    }

    private static processCanvas(sourceCanvas: HTMLCanvasElement, options: any): ImagePiece[] {
        const width = sourceCanvas.width;
        const height = sourceCanvas.height;

        const vPieces =
            options.direction !== 1
                ? this.splitSize(height, options.vertical.quantity)
                : [{ from: 0, to: height }];

        const hPieces =
            options.direction !== 0
                ? this.splitSize(width, options.horizontal.quantity)
                : [{ from: 0, to: width }];

        const results: ImagePiece[] = [];

        vPieces.forEach((v, row) => {
            hPieces.forEach((h, col) => {
                const cropCanvas = document.createElement('canvas');
                cropCanvas.width = h.to - h.from;
                cropCanvas.height = v.to - v.from;

                const ctx = cropCanvas.getContext('2d')!;
                ctx.drawImage(
                    sourceCanvas,
                    h.from,
                    v.from,
                    cropCanvas.width,
                    cropCanvas.height,
                    0,
                    0,
                    cropCanvas.width,
                    cropCanvas.height
                );

                const dataURL = cropCanvas.toDataURL(options.format.type, options.format.quality);

                results.push({
                    dataURL,
                    row: row + 1,
                    col: col + 1,
                    width: cropCanvas.width,
                    height: cropCanvas.height,
                    x: h.from,
                    y: v.from,
                    type: this.getExt(dataURL),
                    downloaded: false,
                });
            });
        });

        return results;
    }

    private static splitSize(total: number, quantity: number) {
        const size = Math.floor(total / quantity);
        const pieces = [];

        let current = 0;
        for (let i = 0; i < quantity; i++) {
            const next = i === quantity - 1 ? total : current + size;
            pieces.push({ from: current, to: next });
            current = next;
        }
        return pieces;
    }

    private static getExt(dataURL: string): string {
        const match = dataURL.match(/^data:image\/(\w+);/);
        return match ? match[1] : 'png';
    }
}
