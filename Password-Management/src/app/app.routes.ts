import { Routes } from '@angular/router';
import { MainLayout } from './shared/components/main-layout/main-layout';
import { Home } from './pages/home/home';
import { AddAccount } from './pages/add-account/add-account';
import { Settings } from './pages/settings/settings';
import { AuthLayout } from './shared/components/auth-layout/auth-layout';
import { Login } from './pages/login/login';
import { Config } from './pages/config/config';
import { AUTH_ROUTE, AuthRoutes, MAIN_ROUTE, MainRoutes } from './core/enums/routes.enum';
import { EditAccount } from './pages/edit-account/edit-account';
import { AddPasscode } from './pages/add-passcode/add-passcode';

export const routes: Routes = [
    {
        path: MAIN_ROUTE,
        component: MainLayout,
        children: [
            {
                path: MainRoutes.Home,
                component: Home,
            },
            {
                path: MainRoutes.AddAccount,
                component: AddAccount,
            },
            {
                path: MainRoutes.Settings,
                component: Settings,
            },
            {
                path: MainRoutes.EditAccount,
                component: EditAccount,
            },
            { path: '', redirectTo: MainRoutes.Home, pathMatch: 'full' },
        ],
    },
    {
        path: AUTH_ROUTE,
        component: AuthLayout,
        children: [
            {
                path: AuthRoutes.Login,
                component: Login,
            },
            {
                path: AuthRoutes.Config,
                component: Config,
            },
            {
                path: AuthRoutes.AddPasscode,
                component: AddPasscode,
            },
            { path: '', redirectTo: AuthRoutes.Config, pathMatch: 'full' },
        ],
    },
    { path: '**', redirectTo: AUTH_ROUTE },
];
