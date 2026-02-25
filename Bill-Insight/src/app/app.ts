import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LoadingDialog } from './shared/components/ui/loading-dialog/loading-dialog';
import { DialogService } from './shared/services/dialog-service';
import { NoticeDialog } from './shared/components/ui/notice-dialog/notice-dialog';
import { AppFolderHelper } from './shared/helpers/app-folder';
import { EAppFolderNames } from './core/enums/folder-names';
import { join } from '@tauri-apps/api/path';
import { EConfigFileNames } from './core/enums/file-names';
import { exists } from '@tauri-apps/plugin-fs';
import { FileHelper } from './shared/helpers/file-helper';
import { SpreadsheetConfigModel } from './shared/models/spreadsheet_config';
import { SpreadsheetConfigStore } from './shared/stores/config-store';
import { LanguageService } from './shared/services/language-service';
import { StoreHelper } from './shared/helpers/store-helper';
import { SettingKeys } from './core/enums/setting-keys';

@Component({
    selector: 'app-root',
    imports: [RouterOutlet, LoadingDialog, NoticeDialog],
    templateUrl: './app.html',
    styleUrl: './app.scss',
})
export class App {
    protected readonly title = signal('BillInsight');
    spreadsheetConfigStore = inject(SpreadsheetConfigStore);

    constructor(public dialogService: DialogService, private languageService: LanguageService) {}

    ngOnInit() {
        this.checkConfig();
        this.init();
    }

    ngAfterViewInit() {}

    private async checkConfig(): Promise<boolean> {
        const credentialFolder = await AppFolderHelper.getFolderPath(EAppFolderNames.CredentialDir);
        const configFolder = await AppFolderHelper.getFolderPath(EAppFolderNames.ConfigDir);

        const credentialPath = await join(
            credentialFolder,
            EConfigFileNames.GOOGLE_CREDENTIAL_FILE_NAME
        );
        const configPath = await join(configFolder, EConfigFileNames.CONFIG_PATH);

        const credentialPathExists = await exists(credentialPath);
        const configPathExists = await exists(configPath);

        const spreadsheetConfig = await FileHelper.getObjectFromFile<SpreadsheetConfigModel>(
            configPath
        );
        if (spreadsheetConfig) {
            this.spreadsheetConfigStore.update(spreadsheetConfig);
        }

        return credentialPathExists && configPathExists;
    }

    private async init() {
        console.log(123);

        await StoreHelper.init();

        const code = await StoreHelper.getValue<string>(SettingKeys.language);
        console.log(code);

        if (!code) {
            return;
        }
        this.languageService.use(code);
    }
}
