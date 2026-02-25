import { Component, inject, signal } from '@angular/core';
import {
    FormBuilder,
    FormGroup,
    FormsModule,
    ReactiveFormsModule,
    Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { AUTH_ROUTE, AuthRoutes } from '../../core/enums/routes.enum';
import { SpreadSheetHelper } from '../../shared/helpers/spread-sheet-helper';
import { SpreadsheetConfigModel } from '../../core/models/spreadsheet-config';
import { SpreadsheetConfigService } from '../../shared/services/config-service';
import { AppFolderHelper } from '../../shared/helpers/app-folder';
import { EAppFolderNames } from '../../core/enums/folder-names';
import { join } from '@tauri-apps/api/path';
import { exists } from '@tauri-apps/plugin-fs';
import { EConfigFileNames } from '../../core/enums/file-names';
import { FileHelper } from '../../shared/helpers/file-helper';
import { SpreadsheetConfigStore } from '../../shared/stores/spread-sheet-config-store';
import { TauriCommandSerivce } from '../../shared/services/tauri-command-service';
import { TextInput } from '../../shared/components/text-input/text-input';
import { FileInput } from '../../shared/components/file-input/file-input';

@Component({
    selector: 'app-config',
    imports: [FormsModule, TranslatePipe, ReactiveFormsModule, TextInput, FileInput],
    templateUrl: './config.html',
    styleUrl: './config.scss',
})
export class Config {
    selectedFile: File | null = null;
    configForm!: FormGroup;
    submitted: boolean = false;
    spreadsheetConfigStore = inject(SpreadsheetConfigStore);
    initial = signal<boolean>(false);

    constructor(
        private router: Router,
        private fb: FormBuilder,
        private configService: SpreadsheetConfigService,
        private tauriCommandSerivce: TauriCommandSerivce
    ) {}

    ngOnInit() {
        try {
            this.configForm = this.fb.group({
                spreadSheetUrl: ['', [Validators.required]],
                spreadSheetId: ['', [Validators.required]],
            });
            this.init();
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
            this.router.navigateByUrl(`/${AUTH_ROUTE}/${AuthRoutes.Config}`);
            return;
        }

        const checkInit = await this.initGoogleSheetService();
        if (checkInit) {
            this.router.navigateByUrl(`/${AUTH_ROUTE}/${AuthRoutes.Login}`);
            return;
        }
        this.router.navigateByUrl(`/${AUTH_ROUTE}/${AuthRoutes.Config}`);
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
}
