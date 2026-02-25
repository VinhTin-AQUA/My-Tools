import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SelectBox, SelectOption } from '../../shared/components/select-box/select-box';
import { AppStore } from '../../shared/stores/app.store';
import { ThemeConsts } from '../../core/consts/themes.const';
import { StoreHelper } from '../../shared/helpers/store-helper';
import { SettingKeys } from '../../core/enums/setting-keys';

@Component({
    selector: 'app-settings',
    imports: [FormsModule, SelectBox],
    templateUrl: './settings.html',
    styleUrl: './settings.scss',
})
export class Settings {
    // Language list
    languages: SelectOption[] = [
        { label: 'English', value: 'en', icon: '🇯🇵' },
        { label: 'Vietnamese', value: 'vi', icon: '🇻🇳' },
        { label: 'Japanese', value: 'jp', icon: '🇺🇲' },
    ];

    // Theme list
    themes: SelectOption[] = [
        { label: ThemeConsts.Light.name, value: ThemeConsts.Light.value, icon: '⬜' },
        { label: ThemeConsts.Dark.name, value: ThemeConsts.Dark.value, icon: '⬛' },
        { label: ThemeConsts.Dracula.name, value: ThemeConsts.Dracula.value, icon: '🟪' },
        { label: ThemeConsts.Monokai.name, value: ThemeConsts.Monokai.value, icon: '🟩' },
        { label: ThemeConsts.GithubLight.name, value: ThemeConsts.GithubLight.value, icon: '🟩' },
        { label: ThemeConsts.GithubDark.name, value: ThemeConsts.GithubDark.value, icon: '🟩' },
    ];

    // Selected
    selectedLanguage = this.languages[0];

    // Toggle states
    openLanguage = false;
    openTheme = false;
    appStore = inject(AppStore);

    ngOnInit() {}

    toggleLanguage() {
        this.openLanguage = !this.openLanguage;
        this.openTheme = false;
    }

    toggleTheme() {
        this.openTheme = !this.openTheme;
        this.openLanguage = false;
    }

    selectLanguage(lang: any) {
        this.selectedLanguage = lang;
        this.openLanguage = false;
    }

    async selectTheme(theme: any) {
        this.appStore.updateTheme(theme);
        await StoreHelper.setValue(SettingKeys.Theme, theme);
        this.openTheme = false;
    }

    exportToExcel() {}
}
