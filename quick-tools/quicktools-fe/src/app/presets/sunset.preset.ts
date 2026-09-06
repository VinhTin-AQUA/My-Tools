import Material from '@openng/optimus-ui-themes/material';
import { definePreset } from '@openng/optimus-ui-themes';

export const SunsetPreset = definePreset(Material, {
    primitive: {
        orange: {
            50: '#FDF0E8',
            100: '#FBE1D1',
            200: '#F7C3A3',
            300: '#F3A575',
            400: '#EF8747',
            500: '#FF7B24',
            600: '#E66A1E',
            700: '#CC5918',
            800: '#B34812',
            900: '#99370C',
            950: '#662506',
        },

        coral: {
            50: '#F8EDF0',
            100: '#F1DBE1',
            200: '#E3B7C3',
            300: '#D593A5',
            400: '#C76F87',
            500: '#FF6B6B',
            600: '#E65A5A',
            700: '#CC4949',
            800: '#B33838',
            900: '#992727',
            950: '#661A1A',
        },

        gold: {
            50: '#F0F5E8',
            100: '#E1EAD1',
            200: '#C3D6A3',
            300: '#A5C175',
            400: '#87AD47',
            500: '#D4A373',
            600: '#C08F64',
            700: '#AC7B55',
            800: '#986746',
            900: '#845337',
            950: '#5A3825',
        },

        amber: {
            50: '#FCF8E8',
            100: '#F9F1D1',
            200: '#F3E3A3',
            300: '#EDD575',
            400: '#E7C747',
            500: '#F4A261',
            600: '#DB8F54',
            700: '#C27C47',
            800: '#A9693A',
            900: '#90562D',
            950: '#603A1E',
        },

        red: {
            50: '#FDE8E8',
            100: '#FBD1D1',
            200: '#F7A3A3',
            300: '#F37575',
            400: '#EF4747',
            500: '#E63946',
            600: '#CC333F',
            700: '#B32D38',
            800: '#992731',
            900: '#801C24',
            950: '#551318',
        },

        teal: {
            50: '#E8F0F5',
            100: '#D1E0EA',
            200: '#A3C2D6',
            300: '#75A3C1',
            400: '#4785AD',
            500: '#2A9D8F',
            600: '#24857A',
            700: '#1E6D64',
            800: '#18554E',
            900: '#123D38',
            950: '#0C2925',
        },

        neutral: {
            0: '#1E1A16',
            50: '#2A241F',
            100: '#362E28',
            200: '#423832',
            300: '#4E423C',
            400: '#5A4C46',
            500: '#665650',
            600: '#7A6A64',
            700: '#8E7E78',
            800: '#A2928C',
            900: '#B6A6A0',
            950: '#CABAB4',
            1000: '#F4E8D8',
        },
    },

    semantic: {
        colorScheme: {
            light: {
                primary: {
                    color: '{orange.500}',
                    contrastColor: '{neutral.0}',
                    hoverColor: '{orange.400}',
                    activeColor: '{orange.300}',
                },

                secondary: {
                    color: '{coral.500}',
                    contrastColor: '{neutral.0}',
                    hoverColor: '{coral.400}',
                    activeColor: '{coral.300}',
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
                    hoverColor: '{orange.500}',
                    focusColor: '{orange.500}',
                },

                content: {
                    background: '{neutral.0}',
                    hoverBackground: '{neutral.50}',
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
                    hoverBorderColor: '{orange.500}',
                    focusBorderColor: '{orange.500}',
                    color: '{neutral.1000}',
                    disabledColor: '{neutral.500}',
                    placeholderColor: '{neutral.500}',
                    invalidBorderColor: '{red.500}',
                },

                highlight: {
                    background: '{orange.950}',
                    focusBackground: '{orange.900}',
                    color: '{orange.500}',
                    focusColor: '{orange.400}',
                },

                success: {
                    color: '{gold.500}',
                    contrastColor: '{neutral.0}',
                    hoverColor: '{gold.400}',
                    activeColor: '{gold.300}',
                },

                warning: {
                    color: '{amber.500}',
                    contrastColor: '{neutral.0}',
                    hoverColor: '{amber.400}',
                    activeColor: '{amber.300}',
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
                    color: '{orange.500}',
                    offset: '1px',
                },
            },
        },
    },
});

export default SunsetPreset;
