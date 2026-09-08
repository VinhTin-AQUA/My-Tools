import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root',
})
export class WebuiService {
    constructor() {}

    /**
     * Gọi một hàm WebUI
     * @param functionName Tên function đã bind trong C#
     * @param args Các tham số truyền vào
     * @returns Promise với kết quả từ C#
     */

    call<T = any>(functionName: string, ...args: any[]): Promise<T> {
        return new Promise(async (resolve, reject) => {
            try {
                // Gọi webui.call và await kết quả
                const result = await webui.call(functionName, ...args);

                // Nếu result là string, thử parse JSON
                if (typeof result === 'string') {
                    try {
                        const parsedResult = JSON.parse(result);
                        resolve(parsedResult as T);
                    } catch (parseError) {
                        // Nếu không parse được JSON, trả về nguyên string
                        resolve(result as unknown as T);
                    }
                } else {
                    resolve(result as T);
                }
            } catch (error) {
                reject(error);
            }
        });
    }

    /**
     * Gọi hàm với tham số JSON
     */
    callJson<T = any>(functionName: string, data: any): Promise<T> {
        return this.call<T>(functionName, JSON.stringify(data));
    }

    /**
     * Upload file với chunk
     */
    async uploadFileChunked(file: File, chunkSize: number = 1024 * 1024): Promise<any> {
        const totalChunks = Math.ceil(file.size / chunkSize);

        // 1. Start session
        const startResult = await this.callJson('startUpload', {
            fileName: file.name,
            totalChunks: totalChunks,
        });
        const sessionId = startResult.sessionId;

        // 2. Upload từng chunk
        for (let i = 0; i < totalChunks; i++) {
            const start = i * chunkSize;
            const end = Math.min(start + chunkSize, file.size);
            const chunk = file.slice(start, end);

            const chunkBase64 = await this.readChunkAsBase64(chunk);

            await this.callJson('uploadChunk', {
                sessionId: sessionId,
                chunkIndex: i,
                chunkData: chunkBase64,
                totalChunks: totalChunks,
            });
        }

        // 3. Finish upload
        const finishResult = await this.callJson('finishUpload', {
            sessionId: sessionId,
        });

        return finishResult;
    }

    /**
     * Đọc chunk thành Base64
     */
    private readChunkAsBase64(chunk: Blob): Promise<string> {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.onload = (e: any) => {
                const base64 = e.target.result.split(',')[1];
                resolve(base64);
            };
            reader.onerror = reject;
            reader.readAsDataURL(chunk);
        });
    }
}
