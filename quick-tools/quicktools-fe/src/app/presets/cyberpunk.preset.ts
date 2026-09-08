import Material from '@openng/optimus-ui-themes/material';
import { definePreset } from '@openng/optimus-ui-themes';

export const CyberpunkPreset = definePreset(Material, {
    primitive: {
        magenta: {
            50: '#FFE6FF',
            100: '#FFB3FF',
            200: '#FF80FF',
            300: '#FF4DFF',
            400: '#FF1AFF',
            500: '#FF00FF',
            600: '#CC00CC',
            700: '#990099',
            800: '#660066',
            900: '#330033',
            950: '#1A001A',
        },

        cyan: {
            50: '#E6F0FF',
            100: '#B3D4FF',
            200: '#80B8FF',
            300: '#4D9CFF',
            400: '#1A80FF',
            500: '#00D4FF',
            600: '#00B3CC',
            700: '#009299',
            800: '#006666',
            900: '#003333',
            950: '#001A1A',
        },

        green: {
            50: '#E6FFE6',
            100: '#B3FFB3',
            200: '#80FF80',
            300: '#4DFF4D',
            400: '#1AFF1A',
            500: '#00FF41',
            600: '#00CC34',
            700: '#009927',
            800: '#00661A',
            900: '#00330D',
            950: '#001A06',
        },

        yellow: {
            50: '#FFFDE6',
            100: '#FFF9B3',
            200: '#FFF580',
            300: '#FFF14D',
            400: '#FFED1A',
            500: '#FFEA00',
            600: '#CCBB00',
            700: '#998C00',
            800: '#665E00',
            900: '#332F00',
            950: '#1A1700',
        },

        red: {
            50: '#FFE6E6',
            100: '#FFB3B3',
            200: '#FF8080',
            300: '#FF4D4D',
            400: '#FF1A1A',
            500: '#FF0040',
            600: '#CC0033',
            700: '#990026',
            800: '#66001A',
            900: '#33000D',
            950: '#1A0006',
        },

        blue: {
            50: '#E6F2FF',
            100: '#B3D9FF',
            200: '#80BFFF',
            300: '#4DA6FF',
            400: '#1A8CFF',
            500: '#0066FF',
            600: '#0052CC',
            700: '#003D99',
            800: '#002966',
            900: '#001433',
            950: '#000A1A',
        },

        neutral: {
            0: '#0A0A0A',
            50: '#111111',
            100: '#1A1A1A',
            200: '#222222',
            300: '#2D2D2D',
            400: '#3D3D3D',
            500: '#4D4D4D',
            600: '#666666',
            700: '#808080',
            800: '#999999',
            900: '#B3B3B3',
            950: '#CCCCCC',
            1000: '#FFFFFF',
        },
    },

    semantic: {
        colorScheme: {
            light: {
                primary: {
                    color: '{magenta.500}',
                    contrastColor: '{neutral.1000}',
                    hoverColor: '{magenta.400}',
                    activeColor: '{magenta.300}',
                },

                secondary: {
                    color: '{cyan.500}',
                    contrastColor: '{neutral.0}',
                    hoverColor: '{cyan.400}',
                    activeColor: '{cyan.300}',
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
                    contrastColor: '{neutral.1000}',
                },

                text: {
                    color: '{neutral.1000}',
                    hoverColor: '{neutral.1000}',
                    mutedColor: '{neutral.700}',
                    hoverMutedColor: '{neutral.800}',
                },

                border: {
                    color: '{neutral.300}',
                    hoverColor: '{cyan.500}',
                    focusColor: '{magenta.500}',
                },

                content: {
                    background: '{neutral.100}',
                    hoverBackground: '{neutral.200}',
                    borderColor: '{neutral.300}',
                    color: '{neutral.1000}',
                    contrastColor: '{neutral.0}',
                },

                formField: {
                    background: '{neutral.100}',
                    disabledBackground: '{neutral.200}',
                    filledBackground: '{neutral.50}',
                    filledHoverBackground: '{neutral.100}',
                    borderColor: '{neutral.300}',
                    hoverBorderColor: '{cyan.500}',
                    focusBorderColor: '{cyan.500}',
                    color: '{neutral.1000}',
                    disabledColor: '{neutral.600}',
                    placeholderColor: '{neutral.500}',
                    invalidBorderColor: '{red.500}',
                },

                highlight: {
                    background: '{magenta.950}',
                    focusBackground: '{magenta.900}',
                    color: '{magenta.500}',
                    focusColor: '{cyan.500}',
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
                    contrastColor: '{neutral.1000}',
                    hoverColor: '{red.400}',
                    activeColor: '{red.300}',
                },

                info: {
                    color: '{blue.500}',
                    contrastColor: '{neutral.1000}',
                    hoverColor: '{blue.400}',
                    activeColor: '{blue.300}',
                },

                focusRing: {
                    width: '2px',
                    style: 'solid',
                    color: '{cyan.500}',
                    offset: '1px',
                },
            },
        },
    },
});

export default CyberpunkPreset;
