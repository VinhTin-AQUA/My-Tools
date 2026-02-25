import { Injectable } from '@angular/core';
import { invoke } from '@tauri-apps/api/core';
import { DialogService } from './dialog-service';
import { Commands } from '../enums/commands.enum';

@Injectable({
    providedIn: 'root',
})
export class TauriCommandService {
    constructor(private dialogService: DialogService) {}

    async invokeCommand<T>(
        command: Commands,
        params: any,
        isLoading: boolean = false
    ): Promise<T | null> {
        if (isLoading) {
            this.dialogService.showLoadingDialog(true);
        }
        try {
            const initOk = await invoke<T>(command, params);

            if (isLoading) {
                this.dialogService.showLoadingDialog(false);
            }
            return initOk;
        } catch (e) {
            console.log(e);
            alert(e);
            if (isLoading) {
                this.dialogService.showLoadingDialog(false);
            }
            return null;
        }
    }
}
