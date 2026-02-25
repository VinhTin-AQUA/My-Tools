import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SelectBox, SelectOption } from '../../shared/components/select-box/select-box';

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
        { label: 'Light', value: 'light', icon: '⬜' },
        { label: 'Dark', value: 'dark', icon: '⬛' },
        { label: 'Blue', value: 'blue', icon: '🟦' },
        { label: 'Purple', value: 'purple', icon: '🟪' },
    ];

    // Selected
    selectedLanguage = this.languages[0];
    selectedTheme = this.themes[0];

    // Toggle states
    openLanguage = false;
    openTheme = false;

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

    selectTheme(theme: any) {
        this.selectedTheme = theme;
        this.openTheme = false;
    }

    exportToExcel() {}
}
