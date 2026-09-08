import Material from '@openng/optimus-ui-themes/material';
import { definePreset } from '@openng/optimus-ui-themes';

export const NordPreset = definePreset(Material, {
    primitive: {
        blue: {
            50: '#EBF0F7',
            100: '#D8E1EF',
            200: '#B8CBE3',
            300: '#98B5D7',
            400: '#789FCB',
            500: '#5E81AC',
            600: '#4C6E92',
            700: '#3B5A79',
            800: '#2A4760',
            900: '#1A3347',
            950: '#0E1F2E',
        },

        cyan: {
            50: '#F0F4F8',
            100: '#E2EAF2',
            200: '#C5D6E6',
            300: '#A8C2D9',
            400: '#8BAECD',
            500: '#81A1C1',
            600: '#6D89A4',
            700: '#597187',
            800: '#455A6A',
            900: '#31424D',
            950: '#1E2B33',
        },

        green: {
            50: '#F0F7EC',
            100: '#E1F0D9',
            200: '#C4E2B3',
            300: '#A6D38D',
            400: '#89C567',
            500: '#A3BE8C',
            600: '#8AA675',
            700: '#728E5E',
            800: '#597647',
            900: '#415E30',
            950: '#2A401E',
        },

        yellow: {
            50: '#FDF8EC',
            100: '#FBF1D9',
            200: '#F7E4B3',
            300: '#F3D68D',
            400: '#EFC967',
            500: '#EBCB8B',
            600: '#D4B07A',
            700: '#BD9569',
            800: '#A67A58',
            900: '#8F5F47',
            950: '#5E3F2E',
        },

        red: {
            50: '#FDF0F0',
            100: '#FCE1E1',
            200: '#F9C4C4',
            300: '#F6A6A6',
            400: '#F38989',
            500: '#BF616A',
            600: '#A3525A',
            700: '#87434A',
            800: '#6B343A',
            900: '#4F252A',
            950: '#33171A',
        },

        teal: {
            50: '#EDF7FA',
            100: '#DCF0F5',
            200: '#B9E1EB',
            300: '#96D2E1',
            400: '#73C3D7',
            500: '#88C0D0',
            600: '#74A6B4',
            700: '#608C98',
            800: '#4C727C',
            900: '#385860',
            950: '#24383E',
        },

        neutral: {
            0: '#2E3440',
            50: '#3B4252',
            100: '#434C5E',
            200: '#4C566A',
            300: '#5E677A',
            400: '#70798A',
            500: '#838C9A',
            600: '#96A0AB',
            700: '#A9B3BC',
            800: '#BCC6CD',
            900: '#CFD9DE',
            950: '#E2EAEF',
            1000: '#ECEFF4',
        },
    },

    semantic: {
        colorScheme: {
            light: {
                primary: {
                    color: '{blue.500}',
                    contrastColor: '{neutral.1000}',
                    hoverColor: '{blue.400}',
                    activeColor: '{blue.300}',
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
                    mutedColor: '{neutral.600}',
                    hoverMutedColor: '{neutral.700}',
                },

                border: {
                    color: '{neutral.300}',
                    hoverColor: '{blue.500}',
                    focusColor: '{blue.500}',
                },

                content: {
                    background: '{neutral.0}',
                    hoverBackground: '{neutral.100}',
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
                    hoverBorderColor: '{blue.500}',
                    focusBorderColor: '{blue.500}',
                    color: '{neutral.1000}',
                    disabledColor: '{neutral.500}',
                    placeholderColor: '{neutral.500}',
                    invalidBorderColor: '{red.500}',
                },

                highlight: {
                    background: '{blue.800}',
                    focusBackground: '{blue.700}',
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
                    color: '{teal.500}',
                    contrastColor: '{neutral.0}',
                    hoverColor: '{teal.400}',
                    activeColor: '{teal.300}',
                },

                focusRing: {
                    width: '2px',
                    style: 'solid',
                    color: '{blue.500}',
                    offset: '1px',
                },
            },
        },
    },
});

export default NordPreset;
