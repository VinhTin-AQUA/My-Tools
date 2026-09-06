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

export const appConfig: ApplicationConfig = {
    providers: [
        provideBrowserGlobalErrorListeners(),
        provideRouter(routes),

        provideOptimus({
            theme: {
                // preset: LightPreset,
                // preset: DarkPreset,
                preset: DraculaPreset,
                // preset: NordPreset,
                // preset: CatppuccinPreset,
                // preset: TokyoNightPreset,
                // preset: CyberpunkPreset,
                // preset: PastelPreset,
                // preset: RetroTerminalPreset,
                // preset: SunsetPreset,

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
