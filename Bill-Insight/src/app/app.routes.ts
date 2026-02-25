import { Routes } from '@angular/router';
import { AuthLayout } from './shared/components/layout/auth-layout/auth-layout';
import { MainLayout } from './shared/components/layout/main-layout/main-layout';
import { AddInvoice } from './pages/add-invoice/add-invoice';
import { ListInvoices } from './pages/list-invoices/list-invoices';
import { Settings } from './pages/settings/settings';
import { ConfigDetails } from './pages/config-details/config-details';
import { AppRouteNames, AuthRouteNames, MainRouteNames } from './core/enums/route-names';

export const routes: Routes = [
    {
        path: AppRouteNames.Auth,
        component: AuthLayout,
        children: [
            {
                path: AuthRouteNames.Config,
                loadComponent: () => import('./pages/config/config').then((m) => m.Config),
            },
            {
                path: '',
                redirectTo: `${AppRouteNames.Auth}/${AuthRouteNames.Config}`,
                pathMatch: 'full',
            },
        ],
    },
    {
        path: AppRouteNames.Main,
        component: MainLayout,
        children: [
            {
                path: MainRouteNames.Home,
                loadComponent: () => import('./pages/home/home').then((m) => m.Home),
            },
            {
                path: MainRouteNames.Invoices,
                component: ListInvoices,
            },
            {
                path: MainRouteNames.ConfigDetails,
                component: ConfigDetails,
            },
            {
                path: MainRouteNames.Settings,
                component: Settings,
            },
            {
                path: MainRouteNames.AddInvoice,
                component: AddInvoice,
            },
            { path: '', redirectTo: '/home', pathMatch: 'full' },
        ],
    },
    { path: '**', redirectTo: `/${AppRouteNames.Auth}/${AuthRouteNames.Config}` },
];
