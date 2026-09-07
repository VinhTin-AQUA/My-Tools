import { inject, Injectable } from '@angular/core';
import { APP_PRESETS, PresetKey } from '../presets/theme-presets';
import { usePreset } from '@openng/optimus-ui-themes';

@Injectable({
    providedIn: 'root',
})
export class ThemeService {
    private currentPreset: PresetKey = 'lightPreset';
    private themeKey: string = 'themeKey';

    get preset(): PresetKey {
        return this.currentPreset;
    }

    init(): void {
        const savedPreset = localStorage.getItem(this.themeKey) as PresetKey | null;

        const preset: PresetKey =
            savedPreset && APP_PRESETS[savedPreset]
                ? savedPreset
                : 'lightPreset';

        const presetConfig = APP_PRESETS[preset];

        if (!presetConfig) {
            return;
        }

        usePreset(presetConfig);
        this.currentPreset = preset;
    }

    setPreset(preset: PresetKey): void {
        if (preset === this.currentPreset) {
            return;
        }

        const presetConfig = APP_PRESETS[preset];

        if (!presetConfig) {
            return;
        }
        usePreset(presetConfig);

        this.currentPreset = preset;
        localStorage.setItem(this.themeKey, preset);
    }
}
