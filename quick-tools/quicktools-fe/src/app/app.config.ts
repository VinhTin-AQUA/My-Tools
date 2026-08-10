import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideOptimus } from '@openng/optimus-ui/config';
import Material from '@openng/optimus-ui-themes/material';
import { definePreset } from '@openng/optimus-ui-themes';

const Noir = definePreset(Material, {
    semantic: {
        // Brand color - Lime
        primary: {
            50: '{green.50}',
            100: '{green.100}',
            200: '{green.200}',
            300: '{green.300}',
            400: '{green.400}',
            500: '{green.500}',
            600: '{green.600}',
            700: '{green.700}',
            800: '{green.800}',
            900: '{green.900}',
            950: '{green.950}',
        },

        colorScheme: {
            light: {
                primary: {
                    color: '{green.700}',
                    inverseColor: '#ffffff',
                    hoverColor: '{green.800}',
                    activeColor: '{green.900}',
                },

                highlight: {
                    background: '{green.100}',
                    focusBackground: '{green.200}',
                    color: '{green.950}',
                    focusColor: '#000000',
                },

                // Surface - nền đen tuyền cho light mode
                surface: {
                    background: '#000000',
                    card: '#0a0a0a',
                    border: '#1a1a1a',
                    hover: '#1a1a1a',
                },

                // Text màu sáng trên nền đen
                text: {
                    color: '#e5e5e5',
                    hoverColor: '#ffffff',
                    mutedColor: '#a3a3a3',
                },
            },

            dark: {
                primary: {
                    color: '{green.400}',
                    inverseColor: '#000000',
                    hoverColor: '{green.300}',
                    activeColor: '{green.200}',
                },

                highlight: {
                    background: '{green.500}',
                    focusBackground: '{green.400}',
                    color: '#000000',
                    focusColor: '#000000',
                },

                // Surface - nền đen tuyền cho dark mode
                surface: {
                    background: '#000000',
                    card: '#0a0a0a',
                    border: '#1a1a1a',
                    hover: '#1a1a1a',
                },

                // Text màu sáng trên nền đen
                text: {
                    color: '#e5e5e5',
                    hoverColor: '#ffffff',
                    mutedColor: '#a3a3a3',
                },
            },
        },
    },
});

export const appConfig: ApplicationConfig = {
    providers: [
        provideBrowserGlobalErrorListeners(),
        provideRouter(routes),
        provideOptimus({
            theme: {
                preset: Noir,
                options: {
                    prefix: 'p',
                    cssLayer: false,
                    darkModeSelector: '.dark', // 'system'
                },
            },
            ripple: true,
        }),
    ],
};
