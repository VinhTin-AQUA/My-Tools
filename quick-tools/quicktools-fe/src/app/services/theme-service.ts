import { inject, Injectable } from '@angular/core';
import { APP_PRESETS, PresetKey } from '../presets/theme-presets';
import { usePreset } from '@openng/optimus-ui-themes';

@Injectable({
    providedIn: 'root',
})
export class ThemeService {
    private currentPreset: PresetKey = 'lightPreset';

    get preset(): PresetKey {
        return this.currentPreset;
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
    }
}
