import Material from '@openng/optimus-ui-themes/material';
import { definePreset } from '@openng/optimus-ui-themes';

export const LightPreset = definePreset(Material, {
    primitive: {
        blue: {
            50: '#E6F0FA',
            100: '#CDE2F5',
            200: '#9BC5EB',
            300: '#69A8E1',
            400: '#378BD7',
            500: '#3B82F6',
            600: '#2563EB',
            700: '#1D4ED8',
            800: '#1E40AF',
            900: '#1E3A8A',
            950: '#172554',
        },
        pink: {
            50: '#FDF0F9',
            100: '#FBE1F3',
            200: '#F7C3E7',
            300: '#F3A5DB',
            400: '#EF87CF',
            500: '#EC4899',
            600: '#DB2777',
            700: '#BE185D',
            800: '#9D174D',
            900: '#831843',
            950: '#500724',
        },
        green: {
            50: '#F0F9EE',
            100: '#E1F3DD',
            200: '#C3E7BB',
            300: '#A5DB99',
            400: '#87CF77',
            500: '#22C55E',
            600: '#16A34A',
            700: '#15803D',
            800: '#166534',
            900: '#14532D',
            950: '#052E16',
        },
        yellow: {
            50: '#FEFCF0',
            100: '#FDF9E1',
            200: '#FBF3C3',
            300: '#F9EDA5',
            400: '#F7E787',
            500: '#EAB308',
            600: '#CA8A04',
            700: '#A16207',
            800: '#854D0E',
            900: '#713F12',
            950: '#422006',
        },
        red: {
            50: '#FDF0F3',
            100: '#FBE1E7',
            200: '#F7C3CF',
            300: '#F3A5B7',
            400: '#EF879F',
            500: '#EF4444',
            600: '#DC2626',
            700: '#B91C1C',
            800: '#991B1B',
            900: '#7F1D1D',
            950: '#450A0A',
        },
        cyan: {
            50: '#EDF9FC',
            100: '#DCF3F9',
            200: '#B9E7F3',
            300: '#96DBED',
            400: '#73CFE7',
            500: '#06B6D4',
            600: '#0891B2',
            700: '#0E7490',
            800: '#155E75',
            900: '#164E63',
            950: '#083344',
        },
        neutral: {
            0: '#FFFFFF', // Nền trắng
            50: '#F8FAFC',
            100: '#F1F5F9',
            200: '#E2E8F0',
            300: '#CBD5E1',
            400: '#94A3B8',
            500: '#64748B',
            600: '#475569',
            700: '#334155',
            800: '#1E293B',
            900: '#0F172A', // ← SỬA: Chữ tối, KHÔNG phải trắng
            950: '#020617',
            1000: '#000000',
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
                    hoverColor: '{blue.600}',
                    activeColor: '{blue.700}',
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
                    hoverColor: '{pink.600}',
                    activeColor: '{pink.700}',
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
                    color: '{neutral.0}', // #FFFFFF - nền trắng
                    contrastColor: '{neutral.900}', // #0F172A - chữ tối
                },

                text: {
                    color: '{neutral.900}', // #0F172A - chữ tối
                    hoverColor: '{neutral.950}',
                    mutedColor: '{neutral.500}',
                    hoverMutedColor: '{neutral.600}',
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
                    disabledColor: '{neutral.400}',
                    placeholderColor: '{neutral.400}',

                    invalidBorderColor: '{danger.color}',
                },

                highlight: {
                    background: '{blue.100}',
                    focusBackground: '{blue.200}',
                    color: '{blue.900}',
                    focusColor: '{blue.900}',
                },

                success: {
                    color: '{green.500}',
                    contrastColor: '{neutral.0}',
                    hoverColor: '{green.600}',
                    activeColor: '{green.700}',
                },

                warning: {
                    color: '{yellow.500}',
                    contrastColor: '{neutral.0}',
                    hoverColor: '{yellow.600}',
                    activeColor: '{yellow.700}',
                },

                danger: {
                    color: '{red.500}',
                    contrastColor: '{neutral.0}',
                    hoverColor: '{red.600}',
                    activeColor: '{red.700}',
                },

                info: {
                    color: '{cyan.500}',
                    contrastColor: '{neutral.0}',
                    hoverColor: '{cyan.600}',
                    activeColor: '{cyan.700}',
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

export default LightPreset;
