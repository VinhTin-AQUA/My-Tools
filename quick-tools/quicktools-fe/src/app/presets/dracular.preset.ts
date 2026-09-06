import Material from '@openng/optimus-ui-themes/material';
import { definePreset } from '@openng/optimus-ui-themes';

export const DraculaPreset = definePreset(Material, {
    primitive: {
        purple: {
            50: '#F4ECFE',
            100: '#E8D9FD',
            200: '#D9C1FB',
            300: '#CAAAFA',
            400: '#C09EF9',
            500: '#BD93F9',
            600: '#AE7FF0',
            700: '#9B6DE0',
            800: '#875BC8',
            900: '#6D45A8',
            950: '#4B2F78',
        },

        pink: {
            50: '#FFF0FA',
            100: '#FFDDF3',
            200: '#FFC4E9',
            300: '#FFA9DE',
            400: '#FF8FD2',
            500: '#FF79C6',
            600: '#F06CAF',
            700: '#D85D9C',
            800: '#B94C86',
            900: '#923968',
            950: '#682749',
        },

        green: {
            50: '#E9FFF0',
            100: '#C9FFD8',
            200: '#A7FFC1',
            300: '#82FFA7',
            400: '#66FA91',
            500: '#50FA7B',
            600: '#45E66E',
            700: '#38CC5D',
            800: '#2DA84D',
            900: '#237F3B',
            950: '#175528',
        },

        yellow: {
            50: '#FFFFF0',
            100: '#FFFED0',
            200: '#FFFDB0',
            300: '#FFFBA0',
            400: '#F9FA9A',
            500: '#F1FA8C',
            600: '#E0E97D',
            700: '#C9D06D',
            800: '#A8AE5B',
            900: '#858B49',
            950: '#5E6335',
        },

        red: {
            50: '#FFECEC',
            100: '#FFD0D0',
            200: '#FFB0B0',
            300: '#FF9090',
            400: '#FF7070',
            500: '#FF5555',
            600: '#F04747',
            700: '#D93B3B',
            800: '#B93232',
            900: '#912727',
            950: '#681C1C',
        },

        cyan: {
            50: '#EDFCFF',
            100: '#D5F8FF',
            200: '#B9F2FF',
            300: '#9CEEFF',
            400: '#8BE9FD',
            500: '#8BE9FD',
            600: '#78D9EC',
            700: '#63C6D8',
            800: '#4EA9BA',
            900: '#3A8797',
            950: '#285F6B',
        },

        neutral: {
            0: '#191A21',
            50: '#21222C',
            100: '#282A36',
            200: '#343746',
            300: '#3B3D4A',
            400: '#44475A',
            500: '#6272A4',
            600: '#A4A4A0',
            700: '#BFBFB9',
            800: '#D0D0CA',
            900: '#E8E8E2',
            950: '#F0F0EA',
            1000: '#F8F8F2',
        },
    },

    semantic: {
        colorScheme: {
            light: {
                primary: {
                    color: '{purple.500}',
                    contrastColor: '{neutral.0}',
                    hoverColor: '{purple.400}',
                    activeColor: '{purple.300}',
                },

                secondary: {
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
                    color: '{neutral.100}',
                    contrastColor: '{neutral.1000}',
                },

                text: {
                    color: '{neutral.1000}',
                    hoverColor: '{neutral.1000}',
                    mutedColor: '{neutral.500}',
                    hoverMutedColor: '{neutral.600}',
                },

                border: {
                    color: '{neutral.400}',
                    hoverColor: '{purple.500}',
                    focusColor: '{purple.500}',
                },

                content: {
                    background: '{neutral.100}',
                    hoverBackground: '{neutral.400}',
                    borderColor: '{neutral.400}',
                    color: '{neutral.1000}',
                    contrastColor: '{neutral.0}',
                },

                formField: {
                    background: '{neutral.100}',
                    disabledBackground: '{neutral.400}',
                    filledBackground: '{neutral.50}',
                    filledHoverBackground: '{neutral.400}',
                    borderColor: '{neutral.400}',
                    hoverBorderColor: '{purple.500}',
                    focusBorderColor: '{purple.500}',
                    color: '{neutral.1000}',
                    disabledColor: '{neutral.500}',
                    placeholderColor: '{neutral.500}',
                    invalidBorderColor: '{red.500}',
                },

                highlight: {
                    background: '{neutral.400}',
                    focusBackground: '{neutral.500}',
                    color: '{neutral.1000}',
                    focusColor: '{neutral.1000}',
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
                    color: '{cyan.500}',
                    contrastColor: '{neutral.0}',
                    hoverColor: '{cyan.400}',
                    activeColor: '{cyan.300}',
                },

                focusRing: {
                    width: '2px',
                    style: 'solid',
                    color: '{purple.500}',
                    offset: '1px',
                },
            },
        },
    },
});

export default DraculaPreset;
