import { Component, inject, signal } from '@angular/core';
import { TauriCommandSerivce } from '../../shared/services/tauri/tauri-command-service';
import { Router } from '@angular/router';
import { SpreadsheetConfigService } from '../../shared/services/config-service';
import { AppFolderHelper } from '../../shared/helpers/app-folder';
import { EAppFolderNames } from '../../core/enums/folder-names';
import { EConfigFileNames } from '../../core/enums/file-names';
import { join } from '@tauri-apps/api/path';
import { exists } from '@tauri-apps/plugin-fs';
import { ReactiveFormsModule, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { SpreadSheetHelper } from '../../shared/helpers/spread-sheet';
import { SpreadsheetConfigModel } from '../../shared/models/spreadsheet_config';
import { RouteNavigationHelper } from '../../shared/helpers/route-navigation-helper';
import { SpreadsheetConfigStore } from '../../shared/stores/config-store';
import { FileHelper } from '../../shared/helpers/file-helper';

@Component({
    selector: 'app-config',
    imports: [ReactiveFormsModule],
    templateUrl: './config.html',
    styleUrl: './config.scss',
})
export class Config {
    selectedFile: File | null = null;
    configForm!: FormGroup;
    submitted = false;
    initial = signal<boolean>(false);

    spreadsheetConfigStore = inject(SpreadsheetConfigStore);

    constructor(
        private tauriCommandSerivce: TauriCommandSerivce,
        private configService: SpreadsheetConfigService,
        private router: Router,
        private fb: FormBuilder
    ) {}

    ngOnInit() {
        try {
            this.init(); 
            this.initForm();
            this.initial.set(true);
        } catch (e) {
            alert(e);
        }
    }

    async saveConfig() {
        this.submitted = true;
        if (!this.configForm.valid) {
            return;
        }

        if (this.selectedFile) {
            await this.configService.saveCredentialFile(this.selectedFile);
        }

        const configModel: SpreadsheetConfigModel = {
            spreadSheetId: this.configForm.controls['spreadSheetId'].value,
            spreadSheetUrl: this.configForm.controls['spreadSheetUrl'].value,
            workingSheet: {
                id: -1,
                isActive: false,
                title: '',
            },
        };

        await this.configService.saveConfig(configModel);
        await this.init();
    }

    onSelectFile(event: any) {
        const file = event.target.files[0];
        this.selectedFile = file;
    }

    onSpreadSheetUrlChange(event: Event) {
        const inputValue = (event.target as HTMLInputElement).value;
        const id = SpreadSheetHelper.extractSpreadsheetId(inputValue);
        this.configForm.controls['spreadSheetId'].setValue(id);
    }

    /* private methods */
    private async init() {
        const checkFileExists = await this.checkConfig();
        if (!checkFileExists) {
            return;
        }

        const checkInit = await this.initGoogleSheetService();
        if (!checkInit) {
            return;
        }

        this.router.navigate(RouteNavigationHelper.MAIN.home);
    }

    private async initGoogleSheetService(): Promise<boolean> {
        const credentialFolder = await AppFolderHelper.getFolderPath(EAppFolderNames.CredentialDir);
        const credentialPath = await join(
            credentialFolder,
            EConfigFileNames.GOOGLE_CREDENTIAL_FILE_NAME
        );

        const r = await this.tauriCommandSerivce.invokeCommand<any>(
            TauriCommandSerivce.INIT_GOOGLE_SHEET_COMMAND,
            { jsonPath: credentialPath }
        );

        return r !== null;
    }

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

    private initForm() {
        this.configForm = this.fb.group({
            spreadSheetUrl: ['', [Validators.required]],
            spreadSheetId: ['', [Validators.required]],
        });
    }
}
