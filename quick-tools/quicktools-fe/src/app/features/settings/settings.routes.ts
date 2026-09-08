import { Routes } from '@angular/router';
import { Settings } from './settings';
import { Appearance } from './pages/appearance/appearance';
import { Configs } from './pages/configs/configs';

export interface SettingsGroup {
    id: string;
    label: string;
    icon: string;
    description: string;
    route: string;
    components: string[]; // List of component selectors
}

export const SETTINGS_GROUPS: SettingsGroup[] = [
    {
        id: 'appearance',
        label: 'Appearance',
        icon: 'pi-palette',
        description: 'Customize the look and feel of your interface',
        route: '/settings/appearance',
        components: ['app-theme-settings', 'app-font-settings', 'app-color-settings'],
    },
    {
        id: 'configs',
        label: 'Configs',
        icon: 'pi-cog',
        description: 'Manage your configs',
        route: '/settings/configs',
        components: ['app-language-settings', 'app-timezone-settings'],
    },
];

export const SETTINGS_ROUTES: Routes = [
    {
        path: '',
        component: Settings,
        children: [
            {
                path: 'appearance',
                component: Appearance,
                data: { groupId: 'appearance' },
            },
            {
                path: 'configs',
                component: Configs,
                data: { groupId: 'configs' },
            },
            {
                path: '',
                redirectTo: 'appearance',
                pathMatch: 'full',
            },
        ],
    },
];
