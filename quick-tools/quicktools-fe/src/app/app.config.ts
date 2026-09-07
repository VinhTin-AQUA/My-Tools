import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideOptimus } from '@openng/optimus-ui/config';
import LightPreset from './presets/light.preset';
import DarkPreset from './presets/dark.preset';
import DraculaPreset from './presets/dracular.preset';
import NordPreset from './presets/nord.preset';
import CatppuccinPreset from './presets/catppuccin.preset';
import TokyoNightPreset from './presets/tokyo-night.preset';
import CyberpunkPreset from './presets/cyberpunk.preset';
import PastelPreset from './presets/pastel.preset';
import RetroTerminalPreset from './presets/retro.preset';
import SunsetPreset from './presets/sunset.preset';
import { APP_PRESETS, PresetKey } from './presets/theme-presets';

export const appConfig: ApplicationConfig = {
    providers: [
        provideBrowserGlobalErrorListeners(),
        provideRouter(routes),

        provideOptimus({
            theme: {
                preset: getSavedPreset(),
                options: {
                    prefix: 'p',
                    cssLayer: false,
                    darkModeSelector: '.light',
                },
            },

            ripple: true,
        }),
    ],
};

function getSavedPreset() {
    const savedPreset = localStorage.getItem('themeKey') as PresetKey | null;

    return savedPreset && APP_PRESETS[savedPreset]
        ? APP_PRESETS[savedPreset]
        : DraculaPreset;
}
