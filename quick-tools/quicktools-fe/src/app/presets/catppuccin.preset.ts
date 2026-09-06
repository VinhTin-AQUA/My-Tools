import Material from '@openng/optimus-ui-themes/material';
import { definePreset } from '@openng/optimus-ui-themes';

export const CatppuccinPreset = definePreset(Material, {
    primitive: {
        blue: {
            50: '#E6F0FA',
            100: '#CDE2F5',
            200: '#9BC5EB',
            300: '#69A8E1',
            400: '#378BD7',
            500: '#89B4FA',
            600: '#6E9CD4',
            700: '#5484AE',
            800: '#3A6C88',
            900: '#205462',
            950: '#143C48',
        },

        pink: {
            50: '#FDF0F9',
            100: '#FBE1F3',
            200: '#F7C3E7',
            300: '#F3A5DB',
            400: '#EF87CF',
            500: '#F5C2E7',
            600: '#D9A6CC',
            700: '#BD8AB1',
            800: '#A16E96',
            900: '#85527B',
            950: '#553652',
        },

        green: {
            50: '#F0F9EE',
            100: '#E1F3DD',
            200: '#C3E7BB',
            300: '#A5DB99',
            400: '#87CF77',
            500: '#A6E3A1',
            600: '#8DC788',
            700: '#74AB6F',
            800: '#5B8F56',
            900: '#42733D',
            950: '#294D26',
        },

        yellow: {
            50: '#FEFCF0',
            100: '#FDF9E1',
            200: '#FBF3C3',
            300: '#F9EDA5',
            400: '#F7E787',
            500: '#F9E2AF',
            600: '#D9C598',
            700: '#B9A881',
            800: '#998B6A',
            900: '#796E53',
            950: '#504A38',
        },

        red: {
            50: '#FDF0F3',
            100: '#FBE1E7',
            200: '#F7C3CF',
            300: '#F3A5B7',
            400: '#EF879F',
            500: '#F38BA8',
            600: '#D07790',
            700: '#AD6378',
            800: '#8A4F60',
            900: '#673B48',
            950: '#442730',
        },

        cyan: {
            50: '#EDF9FC',
            100: '#DCF3F9',
            200: '#B9E7F3',
            300: '#96DBED',
            400: '#73CFE7',
            500: '#89DCEB',
            600: '#74BCC9',
            700: '#5F9CA7',
            800: '#4A7C85',
            900: '#355C63',
            950: '#223C41',
        },

        neutral: {
            0: '#1E1E2E',
            50: '#282838',
            100: '#313244',
            200: '#45475A',
            300: '#585B70',
            400: '#6C6F85',
            500: '#7F849C',
            600: '#9399B2',
            700: '#A6ADC8',
            800: '#BAC2DE',
            900: '#CDD6F4',
            950: '#E5E8F5',
            1000: '#F5F7FA',
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
                    hoverColor: '{neutral.1000}',
                    mutedColor: '{neutral.600}',
                    hoverMutedColor: '{neutral.700}',
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
                    placeholderColor: '{neutral.500}',

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

export default CatppuccinPreset;
