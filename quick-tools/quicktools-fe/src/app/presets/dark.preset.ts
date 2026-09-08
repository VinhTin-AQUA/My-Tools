import Material from '@openng/optimus-ui-themes/material';
import { definePreset } from '@openng/optimus-ui-themes';

// ============================================
// 2. DARK THEME PRESET
// ============================================
export const DarkPreset = definePreset(Material, {
    primitive: {
        blue: {
            50: '#172554',
            100: '#1E3A8A',
            200: '#1E40AF',
            300: '#1D4ED8',
            400: '#2563EB',
            500: '#3B82F6',
            600: '#60A5FA',
            700: '#93C5FD',
            800: '#BFDBFE',
            900: '#DBEAFE',
            950: '#EFF6FF',
        },
        pink: {
            50: '#500724',
            100: '#831843',
            200: '#9D174D',
            300: '#BE185D',
            400: '#DB2777',
            500: '#EC4899',
            600: '#F472B6',
            700: '#F9A8D4',
            800: '#FBCFE8',
            900: '#FCE7F3',
            950: '#FDF2F8',
        },
        green: {
            50: '#052E16',
            100: '#14532D',
            200: '#166534',
            300: '#15803D',
            400: '#16A34A',
            500: '#22C55E',
            600: '#4ADE80',
            700: '#86EFAC',
            800: '#BBF7D0',
            900: '#DCFCE7',
            950: '#F0FDF4',
        },
        yellow: {
            50: '#422006',
            100: '#713F12',
            200: '#854D0E',
            300: '#A16207',
            400: '#CA8A04',
            500: '#EAB308',
            600: '#FACC15',
            700: '#FDE047',
            800: '#FEF08A',
            900: '#FEF9C3',
            950: '#FEFCE8',
        },
        red: {
            50: '#450A0A',
            100: '#7F1D1D',
            200: '#991B1B',
            300: '#B91C1C',
            400: '#DC2626',
            500: '#EF4444',
            600: '#F87171',
            700: '#FCA5A5',
            800: '#FECACA',
            900: '#FEE2E2',
            950: '#FEF2F2',
        },
        cyan: {
            50: '#083344',
            100: '#164E63',
            200: '#155E75',
            300: '#0E7490',
            400: '#0891B2',
            500: '#06B6D4',
            600: '#22D3EE',
            700: '#67E8F9',
            800: '#A5F3FC',
            900: '#CFFAFE',
            950: '#ECFEFF',
        },
        neutral: {
            0: '#0F172A', // Nền tối
            50: '#1E293B',
            100: '#334155',
            200: '#475569',
            300: '#64748B',
            400: '#94A3B8',
            500: '#CBD5E1',
            600: '#E2E8F0',
            700: '#F1F5F9',
            800: '#F8FAFC',
            900: '#FFFFFF', // Chữ trắng
            950: '#F8FAFC',
            1000: '#FFFFFF',
        },
    },

    semantic: {
        colorScheme: {
            light: {
                primary: {
                    50: '{blue.50}',
                    100: '{blue.100}',
                    200: '{blue.200}',
                    300: '{blue.300}',
                    400: '{blue.400}',
                    500: '{blue.500}',
                    600: '{blue.600}',
                    700: '{blue.700}',
                    800: '{blue.800}',
                    900: '{blue.900}',
                    950: '{blue.950}',

                    color: '{blue.500}',
                    contrastColor: '{neutral.0}',
                    hoverColor: '{blue.400}',
                    activeColor: '{blue.300}',
                },

                secondary: {
                    50: '{pink.50}',
                    100: '{pink.100}',
                    200: '{pink.200}',
                    300: '{pink.300}',
                    400: '{pink.400}',
                    500: '{pink.500}',
                    600: '{pink.600}',
                    700: '{pink.700}',
                    800: '{pink.800}',
                    900: '{pink.900}',
                    950: '{pink.950}',

                    color: '{pink.500}',
                    contrastColor: '{neutral.0}',
                    hoverColor: '{pink.400}',
                    activeColor: '{pink.300}',
                },

                surface: {
                    0: '{neutral.0}',
                    50: '{neutral.50}',
                    100: '{neutral.100}',
                    200: '{neutral.200}',
                    300: '{neutral.300}',
                    400: '{neutral.400}',
                    500: '{neutral.500}',
                    600: '{neutral.600}',
                    700: '{neutral.700}',
                    800: '{neutral.800}',
                    900: '{neutral.900}',
                    950: '{neutral.950}',
                },

                background: {
                    color: '{neutral.0}',
                    contrastColor: '{neutral.900}',
                },

                text: {
                    color: '{neutral.900}',
                    hoverColor: '{neutral.950}',
                    mutedColor: '{neutral.400}',
                    hoverMutedColor: '{neutral.500}',
                },

                border: {
                    color: '{neutral.300}',
                    hoverColor: '{neutral.400}',
                    focusColor: '{primary.color}',
                },

                content: {
                    background: '{neutral.100}',
                    hoverBackground: '{neutral.200}',
                    borderColor: '{neutral.300}',
                    color: '{neutral.900}',
                    contrastColor: '{neutral.0}',
                },

                formField: {
                    background: '{neutral.100}',
                    disabledBackground: '{neutral.200}',
                    filledBackground: '{neutral.50}',
                    filledHoverBackground: '{neutral.100}',

                    borderColor: '{neutral.300}',
                    hoverBorderColor: '{primary.color}',
                    focusBorderColor: '{primary.color}',

                    color: '{neutral.900}',
                    disabledColor: '{neutral.500}',
                    placeholderColor: '{neutral.400}',

                    invalidBorderColor: '{danger.color}',
                },

                highlight: {
                    background: '{blue.800}',
                    focusBackground: '{blue.700}',
                    color: '{blue.50}',
                    focusColor: '{blue.50}',
                },

                success: {
                    color: '{green.500}',
                    contrastColor: '{neutral.0}',
                    hoverColor: '{green.400}',
                    activeColor: '{green.300}',
                },

                warning: {
                    color: '{yellow.500}',
                    contrastColor: '{neutral.0}',
                    hoverColor: '{yellow.400}',
                    activeColor: '{yellow.300}',
                },

                danger: {
                    color: '{red.500}',
                    contrastColor: '{neutral.0}',
                    hoverColor: '{red.400}',
                    activeColor: '{red.300}',
                },

                info: {
                    color: '{cyan.500}',
                    contrastColor: '{neutral.0}',
                    hoverColor: '{cyan.400}',
                    activeColor: '{cyan.300}',
                },

                focusRing: {
                    width: '2px',
                    style: 'solid',
                    color: '{primary.color}',
                    offset: '1px',
                },
            },
        },
    },
});

export default DarkPreset;
