import { Routes } from '@angular/router';
import { Home } from './features/home/home';
import { UpscaleImage } from './features/upscale-image/upscale-image';

export const routes: Routes = [
    {
        path: '',
        component: Home,
        title: 'home',
    },
    {
        path: 'upscale-image',
        component: UpscaleImage,
        title: 'Upscale Image',
    },
];
