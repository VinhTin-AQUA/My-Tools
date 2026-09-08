import Material from '@openng/optimus-ui-themes/material';
import { definePreset } from '@openng/optimus-ui-themes';

export const PastelPreset = definePreset(Material, {
    primitive: {
        pink: {
            50: '#FDE8F0',
            100: '#FCD1E1',
            200: '#FAA3C3',
            300: '#F875A5',
            400: '#F64787',
            500: '#FFB3C6',
            600: '#E699B0',
            700: '#CC7F9A',
            800: '#B36584',
            900: '#994B6E',
            950: '#66324A',
        },

        blue: {
            50: '#E8F4FD',
            100: '#D1E9FB',
            200: '#A3D3F7',
            300: '#75BDF3',
            400: '#47A7EF',
            500: '#B5DEFF',
            600: '#9CC4E6',
            700: '#83AACC',
            800: '#6A90B3',
            900: '#517699',
            950: '#364F66',
        },

        green: {
            50: '#EDF9EC',
            100: '#DBF3D9',
            200: '#B7E7B3',
            300: '#93DB8D',
            400: '#6FCF67',
            500: '#B5E6B5',
            600: '#9CCC9C',
            700: '#83B283',
            800: '#6A986A',
            900: '#517E51',
            950: '#365436',
        },

        yellow: {
            50: '#FEFCEC',
            100: '#FDF9D9',
            200: '#FBF3B3',
            300: '#F9ED8D',
            400: '#F7E767',
            500: '#FFF5BA',
            600: '#E6D9A6',
            700: '#CCBD92',
            800: '#B3A17E',
            900: '#99856A',
            950: '#665946',
        },

        red: {
            50: '#FDECED',
            100: '#FBD9DB',
            200: '#F7B3B7',
            300: '#F38D93',
            400: '#EF676F',
            500: '#FFB3B8',
            600: '#E69CA2',
            700: '#CC858C',
            800: '#B36E76',
            900: '#995760',
            950: '#663A40',
        },

        cyan: {
            50: '#E8FAFD',
            100: '#D1F5FB',
            200: '#A3EBF7',
            300: '#75E1F3',
            400: '#47D7EF',
            500: '#B5E8F5',
            600: '#9CCCD9',
            700: '#83B0BD',
            800: '#6A94A1',
            900: '#517885',
            950: '#365059',
        },

        neutral: {
            0: '#FFF8FA',
            50: '#FFF5F7',
            100: '#FFF0F3',
            200: '#FFE8ED',
            300: '#FFDCE3',
            400: '#FFCDD7',
            500: '#F5C4CF',
            600: '#E6B8C2',
            700: '#D4AAB5',
            800: '#BF9CA7',
            900: '#A68E98',
            950: '#8A767F',
            1000: '#6B5A62',
        },
    },

    semantic: {
        colorScheme: {
            light: {
                primary: {
                    color: '{pink.500}',
                    contrastColor: '{neutral.1000}',
                    hoverColor: '{pink.400}',
                    activeColor: '{pink.300}',
                },

                secondary: {
                    color: '{blue.500}',
                    contrastColor: '{neutral.1000}',
                    hoverColor: '{blue.400}',
                    activeColor: '{blue.300}',
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
                    hoverColor: '{neutral.950}',
                    mutedColor: '{neutral.700}',
                    hoverMutedColor: '{neutral.600}',
                },

                border: {
                    color: '{neutral.300}',
                    hoverColor: '{pink.500}',
                    focusColor: '{pink.500}',
                },

                content: {
                    background: '{neutral.0}',
                    hoverBackground: '{neutral.50}',
                    borderColor: '{neutral.300}',
                    color: '{neutral.1000}',
                    contrastColor: '{neutral.0}',
                },

                formField: {
                    background: '{neutral.0}',
                    disabledBackground: '{neutral.100}',
                    filledBackground: '{neutral.50}',
                    filledHoverBackground: '{neutral.100}',
                    borderColor: '{neutral.300}',
                    hoverBorderColor: '{pink.500}',
                    focusBorderColor: '{pink.500}',
                    color: '{neutral.1000}',
                    disabledColor: '{neutral.400}',
                    placeholderColor: '{neutral.400}',
                    invalidBorderColor: '{red.500}',
                },

                highlight: {
                    background: '{pink.50}',
                    focusBackground: '{pink.100}',
                    color: '{neutral.1000}',
                    focusColor: '{neutral.950}',
                },

                success: {
                    color: '{green.500}',
                    contrastColor: '{neutral.1000}',
                    hoverColor: '{green.400}',
                    activeColor: '{green.300}',
                },

                warning: {
                    color: '{yellow.500}',
                    contrastColor: '{neutral.1000}',
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
                    contrastColor: '{neutral.1000}',
                    hoverColor: '{cyan.400}',
                    activeColor: '{cyan.300}',
                },

                focusRing: {
                    width: '2px',
                    style: 'solid',
                    color: '{pink.500}',
                    offset: '1px',
                },
            },
        },
    },
});

export default PastelPreset;
