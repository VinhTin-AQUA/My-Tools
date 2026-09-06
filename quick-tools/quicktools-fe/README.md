# QuicktoolsFe

- https://optimus.openng.org/listbox

## Prompt

```txt
Trong angular,
Sử dụng PrimeNG Theme (styled mode) làm nguồn màu duy nhất cho toàn bộ giao diện.

Toàn bộ màu phải lấy từ PrimeNG CSS variables (--p-*).

Quy tắc sử dụng màu tôi định nghĩa thành nhiều preset, có mẫu như sau

export const NoirLight = definePreset(Material, {
    // primitive: {
    //     green: NoirPalette.green,
    //     red: NoirPalette.red,
    //     noir: NoirPalette.noir,
    // },
    semantic: {
        primary: {
            color: '{green.700}',
            inverseColor: '{noir.0}',
            hoverColor: '{green.800}',
            activeColor: '{green.900}',
        },

        danger: {
            color: '{red.600}',
            inverseColor: '{surface.0}',
            hoverColor: '{red.700}',
            activeColor: '{red.800}',
        },

        highlight: {
            background: '{green.100}',
            focusBackground: '{green.200}',
            color: '{green.950}',
            focusColor: '{surface.950}',
        },

        surface: {
            background: '{surface.50}',
            card: '{surface.0}',
            border: '{surface.200}',
            hover: '{surface.100}',
        },

        text: {
            color: '{surface.900}',
            hoverColor: '{surface.950}',
            mutedColor: '{surface.500}',
        },
    },
});

và giao diện html đồng thời phải sử dụng tailwind class inline html về bố cục, kích thước, .... riêng với màu sắc phải sử dụng trong preset trên và code trong css riêng. Lưu ý cho cả giao diện mobile

Đồng thời phải sử dụng các control hiện đại, ví dụ @for, @if

code giao diện theo mô tả giao diện sau:

tôi đã định nghĩa sẵn các theme như sau
import { NoirDark } from './noir-dark.preset';
import { NoirLight } from './noir-light.preset';

export const APP_PRESETS = {
    noirLight: NoirLight,
    noirDark: NoirDark,
} as const;

export type PresetKey = keyof typeof APP_PRESETS;

interface PresetItem {
    presetKey: PresetKey;
    name: string;
}

export const presets: PresetItem[] = [
    {
        name: 'Light',
        presetKey: 'noirLight',
    },
    {
        name: 'Dark',
        presetKey: 'noirDark',
    },
];

và service

export class ThemeService {
    private currentPreset: PresetKey = 'noirLight';

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


giao diện phải load danh sách presets, chọn 1 preset và sử dụng service để set preset


```