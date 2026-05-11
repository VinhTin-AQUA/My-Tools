import { open } from '@tauri-apps/plugin-dialog';
import { IMAGE_EXTENSIONS } from '../../core/constants/file-extensions';
import { invoke } from '@tauri-apps/api/core';
import { FileMetadata } from '../../core/models/upload-image.model';

export interface SelectedFile {
    name: string;
    path: string;
    size: number;
}

export class DialogHelper {
    static async selectMultiFiles(): Promise<
        SelectedFile[] | null
    > {
        const files = await open({
            multiple: true,
            directory: false,
            filters: [
                {
                    name: 'Image',
                    extensions: IMAGE_EXTENSIONS,
                },
            ],
        });

        if (!files) {
            return null;
        }

        const normalized =
            Array.isArray(files)
                ? files
                : [files];

        const results = await Promise.all(
            normalized.map(async (path) => {
                const metadata =
                    await invoke<FileMetadata>(
                        'get_file_metadata',
                        {
                            path,
                        },
                    );

                return {
                    path,

                    name: metadata.name,

                    size: metadata.size,
                };
            }),
        );

        return results;
    }
}
