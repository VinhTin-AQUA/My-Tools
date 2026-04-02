import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { AppTheme, AppThemeType } from '../models/theme.model';

@Injectable({
    providedIn: 'root',
})
export class ThemeService {
    appThemes = signal<Record<string, AppTheme>>({});

    constructor(private http: HttpClient) {}

    async init() {
        // themes
        const themes = await firstValueFrom(
            this.http.get<Record<string, AppTheme>>('themes/themes.json'),
        );

        this.appThemes.set(themes);
        this.applyThemeToDOM('light');
    }

    private applyThemeToDOM(type: AppThemeType) {
        const root = document.documentElement;
        const selectedTheme: AppTheme = this.appThemes()[type];

        Object.entries(selectedTheme).forEach(([key, value]) => {
            root.style.setProperty(`--${key}`, value);
        });
    }
}
