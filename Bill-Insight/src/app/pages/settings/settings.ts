import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { LanguageService } from '../../shared/services/language-service';
import { StoreHelper } from '../../shared/helpers/store-helper';
import { SettingKeys } from '../../core/enums/setting-keys';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
    selector: 'app-settings',
    imports: [CommonModule, TranslatePipe],
    templateUrl: './settings.html',
    styleUrl: './settings.scss',
})
export class Settings {
    selectedLanguage = 'en';
    selectedTheme = 'light';

    languages: LanguageOption[] = LanguageService.LANGUAGES;

    themes: ThemeOption[] = [
        { value: 'light', label: 'Light', icon: '🌞' },
        { value: 'dark', label: 'Dark', icon: '🌙' },
    ];

    constructor(private languageService: LanguageService) {}

    ngOnInit() {}

    ngAfterViewInit() {
        this.init();
    }

    async changeLanguage(code: string) {
        this.selectedLanguage = code;
        await StoreHelper.setValue(SettingKeys.language, code);
        this.languageService.use(code);
    }

    async changeTheme(theme: string) {}

    private async init() {
        const code = await StoreHelper.getValue<string>(SettingKeys.language);

        if (!code) {
            return;
        }
        this.selectedLanguage = code;
        this.languageService.use(code);
        console.log(code);
            
    }
}
