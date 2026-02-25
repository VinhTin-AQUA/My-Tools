import { Routes } from '@angular/router';
import { MainLayout } from './shared/components/main-layout/main-layout';
import { Home } from './pages/home/home';
import { Settings } from './pages/settings/settings';
import { MAIN_ROUTE, MainRoutes } from './core/enums/routes.enum';
import { IloveimgUpscaleImage } from './pages/iloveimg-upscale-image/iloveimg-upscale-image';
import { SplitImg } from './pages/split-img/split-img';

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
                path: MainRoutes.Settings,
                component: Settings,
            },
            {
                path: MainRoutes.IloveimgUpscaleImage,
                component: IloveimgUpscaleImage,
            },
            {
                path: MainRoutes.SplitImg,
                component: SplitImg,
            },
            { path: '', redirectTo: MainRoutes.Home, pathMatch: 'full' },
        ],
    },
    { path: '**', redirectTo: MAIN_ROUTE },
];
