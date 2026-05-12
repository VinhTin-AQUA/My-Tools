import { open } from '@tauri-apps/plugin-dialog';
import { IMAGE_EXTENSIONS } from '../../core/constants/file-extensions';
import { invoke } from '@tauri-apps/api/core';
import { FileMetadata } from '../../core/models/upload-image.model';
import { readFile } from '@tauri-apps/plugin-fs';
import { Commands } from '../../core/constants/commands.enum';



export class DialogHelper {
    
}
