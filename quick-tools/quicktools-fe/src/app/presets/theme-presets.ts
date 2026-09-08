import CatppuccinPreset from './catppuccin.preset';
import CyberpunkPreset from './cyberpunk.preset';
import DraculaPreset from './dracular.preset';
import { DarkPreset } from './dark.preset';
import { LightPreset } from './light.preset';
import NordPreset from './nord.preset';
import PastelPreset from './pastel.preset';
import RetroTerminalPreset from './retro.preset';
import TokyoNightPreset from './tokyo-night.preset';
import SunsetPreset from './sunset.preset';

export const APP_PRESETS = {
    lightPreset: LightPreset,
    darkPreset: DarkPreset,
    draculaPreset: DraculaPreset,
    nordPreset: NordPreset,
    catppuccinPreset: CatppuccinPreset,
    tokyoNightPreset: TokyoNightPreset,
    cyberpunkPreset: CyberpunkPreset,
    pastelPreset: PastelPreset,
    retroTerminalPreset: RetroTerminalPreset,
    sunsetPreset: SunsetPreset,
} as const;

export type PresetKey = keyof typeof APP_PRESETS;

interface PresetItem {
    presetKey: PresetKey;
    name: string;
}

export const LIST_PRESETS: PresetItem[] = [
    { presetKey: 'lightPreset', name: 'Light' },
    { presetKey: 'darkPreset', name: 'Dark' },
    { presetKey: 'draculaPreset', name: 'Dracula' },
    { presetKey: 'nordPreset', name: 'Nord' },
    { presetKey: 'catppuccinPreset', name: 'Catppuccin' },
    { presetKey: 'tokyoNightPreset', name: 'TokyoNight' },
    { presetKey: 'cyberpunkPreset', name: 'Cyberpunk' },
    { presetKey: 'pastelPreset', name: 'Pastel' },
    { presetKey: 'retroTerminalPreset', name: 'RetroTerminal' },
    { presetKey: 'sunsetPreset', name: 'Sunset' },
];
