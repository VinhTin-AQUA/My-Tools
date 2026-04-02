export const APP_THEMES = {
    Light: 'light',
    Dark: 'dark',
} as const;
export type AppThemeType = (typeof APP_THEMES)[keyof typeof APP_THEMES];

export type AppTheme = {
    foreground: string;
    background: string;
    black: string;
    red: string;
    green: string;
    yellow: string;
    blue: string;
    magenta: string;
    cyan: string;
    white: string;
    brightBlack: string;
    brightRed: string;
    brightGreen: string;
    brightYellow: string;
    brightBlue: string;
    brightMagenta: string;
    brightCyan: string;
    brightWhite: string;
    cursor: string;
    cursorAccent: string;
    highlight: string;
};
