import Material from '@openng/optimus-ui-themes/material';
import { definePreset } from '@openng/optimus-ui-themes';

export const TokyoNightPreset = definePreset(Material, {
    primitive: {
        blue: {
            50: '#EDF4FD',
            100: '#DBE9FB',
            200: '#B7D3F7',
            300: '#93BDF3',
            400: '#6FA7EF',
            500: '#7AA2F7',
            600: '#5E8AD4',
            700: '#4272B1',
            800: '#265A8E',
            900: '#1A4270',
            950: '#0E2A4D',
        },

        purple: {
            50: '#F5F0FD',
            100: '#EBE1FB',
            200: '#D7C3F7',
            300: '#C3A5F3',
            400: '#AF87EF',
            500: '#BB9AF7',
            600: '#9E7ED4',
            700: '#8162B1',
            800: '#64468E',
            900: '#472A70',
            950: '#2E1A4D',
        },

        green: {
            50: '#F0F9EC',
            100: '#E1F3D9',
            200: '#C3E7B3',
            300: '#A5DB8D',
            400: '#87CF67',
            500: '#9ECE6A',
            600: '#84B05A',
            700: '#6A924A',
            800: '#50743A',
            900: '#36562A',
            950: '#22381A',
        },

        orange: {
            50: '#FDF7EC',
            100: '#FBEFD9',
            200: '#F7DFB3',
            300: '#F3CF8D',
            400: '#EFBF67',
            500: '#E0AF68',
            600: '#C4965A',
            700: '#A87D4C',
            800: '#8C643E',
            900: '#704B30',
            950: '#4A3220',
        },

        red: {
            50: '#FDF0F2',
            100: '#FBE1E5',
            200: '#F7C3CB',
            300: '#F3A5B1',
            400: '#EF8797',
            500: '#F7768E',
            600: '#D4667A',
            700: '#B15666',
            800: '#8E4652',
            900: '#6B363E',
            950: '#48262A',
        },

        cyan: {
            50: '#EDF9FD',
            100: '#DBF3FB',
            200: '#B7E7F7',
            300: '#93DBF3',
            400: '#6FCFEF',
            500: '#7DCFFF',
            600: '#68B0D9',
            700: '#5391B3',
            800: '#3E728D',
            900: '#295367',
            950: '#1A3847',
        },

        neutral: {
            0: '#1A1B26',
            50: '#222332',
            100: '#2A2B3E',
            200: '#32334A',
            300: '#3D3E56',
            400: '#484A62',
            500: '#56586E',
            600: '#65677A',
            700: '#747686',
            800: '#838592',
            900: '#A0A2B0',
            950: '#C0C2D0',
            1000: '#C0CAF5',
        },
    },

    semantic: {
        colorScheme: {
            light: {
                primary: {
                    color: '{blue.500}',
                    contrastColor: '{neutral.0}',
                    hoverColor: '{blue.400}',
                    activeColor: '{blue.300}',
                },

                secondary: {
                    color: '{purple.500}',
                    contrastColor: '{neutral.0}',
                    hoverColor: '{purple.400}',
                    activeColor: '{purple.300}',
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
                    mutedColor: '{neutral.900}',
                    hoverMutedColor: '{neutral.950}',
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
                    background: '{neutral.100}',
                    disabledBackground: '{neutral.200}',
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
                    background: '{neutral.200}',
                    focusBackground: '{neutral.300}',
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
                    color: '{orange.500}',
                    contrastColor: '{neutral.0}',
                    hoverColor: '{orange.400}',
                    activeColor: '{orange.300}',
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
                    color: '{blue.500}',
                    offset: '1px',
                },
            },
        },
    },
});

export default TokyoNightPreset;
