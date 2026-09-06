import Material from '@openng/optimus-ui-themes/material';
import { definePreset } from '@openng/optimus-ui-themes';

export const RetroTerminalPreset = definePreset(Material, {
    primitive: {
        green: {
            50: '#E6F0E6',
            100: '#CCE0CC',
            200: '#99C299',
            300: '#66A366',
            400: '#338533',
            500: '#33FF33',
            600: '#2ECC2E',
            700: '#299929',
            800: '#1F661F',
            900: '#143314',
            950: '#0A1A0A',
        },

        cyan: {
            50: '#E6E6F0',
            100: '#CCCCE0',
            200: '#9999C2',
            300: '#6666A3',
            400: '#333385',
            500: '#00CCFF',
            600: '#00A3CC',
            700: '#007A99',
            800: '#005266',
            900: '#002933',
            950: '#00141A',
        },

        success: {
            50: '#E6F5E6',
            100: '#CCEACC',
            200: '#99D699',
            300: '#66C166',
            400: '#33AD33',
            500: '#00FF00',
            600: '#00CC00',
            700: '#009900',
            800: '#006600',
            900: '#003300',
            950: '#001A00',
        },

        yellow: {
            50: '#FDF5E6',
            100: '#FBEACC',
            200: '#F7D699',
            300: '#F3C166',
            400: '#EFAD33',
            500: '#FFCC00',
            600: '#CCA300',
            700: '#997A00',
            800: '#665200',
            900: '#332900',
            950: '#1A1400',
        },

        red: {
            50: '#F5E6E6',
            100: '#EACCCC',
            200: '#D69999',
            300: '#C16666',
            400: '#AD3333',
            500: '#FF0000',
            600: '#CC0000',
            700: '#990000',
            800: '#660000',
            900: '#330000',
            950: '#1A0000',
        },

        blue: {
            50: '#E6F0F5',
            100: '#CCE0EA',
            200: '#99C2D6',
            300: '#66A3C1',
            400: '#3385AD',
            500: '#00BFFF',
            600: '#0099CC',
            700: '#007399',
            800: '#004D66',
            900: '#002633',
            950: '#00131A',
        },

        neutral: {
            0: '#0D0D0D',
            50: '#141414',
            100: '#1A1A1A',
            200: '#222222',
            300: '#2D2D2D',
            400: '#3D3D3D',
            500: '#4D4D4D',
            600: '#808080',
            700: '#A0A0A0',
            800: '#C0C0C0',
            900: '#D0D0D0',
            950: '#E0E0E0',
            1000: '#33FF33',
        },
    },

    semantic: {
        colorScheme: {
            light: {
                primary: {
                    color: '{green.500}',
                    contrastColor: '{neutral.0}',
                    hoverColor: '{green.400}',
                    activeColor: '{green.300}',
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
                    contrastColor: '{green.500}',
                },

                text: {
                    color: '{green.500}',
                    hoverColor: '{green.400}',
                    mutedColor: '{neutral.700}',
                    hoverMutedColor: '{neutral.800}',
                },

                border: {
                    color: '{green.800}',
                    hoverColor: '{green.500}',
                    focusColor: '{cyan.500}',
                },

                content: {
                    background: '{neutral.0}',
                    hoverBackground: '{neutral.100}',
                    borderColor: '{green.800}',
                    color: '{green.500}',
                    contrastColor: '{neutral.0}',
                },

                formField: {
                    background: '{neutral.0}',
                    disabledBackground: '{neutral.200}',
                    filledBackground: '{neutral.50}',
                    filledHoverBackground: '{neutral.100}',
                    borderColor: '{green.800}',
                    hoverBorderColor: '{cyan.500}',
                    focusBorderColor: '{cyan.500}',
                    color: '{green.500}',
                    disabledColor: '{neutral.600}',
                    placeholderColor: '{neutral.600}',
                    invalidBorderColor: '{red.500}',
                },

                highlight: {
                    background: '{neutral.100}',
                    focusBackground: '{neutral.200}',
                    color: '{green.500}',
                    focusColor: '{cyan.500}',
                },

                success: {
                    color: '{success.500}',
                    contrastColor: '{neutral.0}',
                    hoverColor: '{success.400}',
                    activeColor: '{success.300}',
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
                    contrastColor: '{neutral.0}',
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

export default RetroTerminalPreset;
