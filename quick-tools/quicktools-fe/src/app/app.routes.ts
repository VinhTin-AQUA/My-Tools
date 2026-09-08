import { Routes } from '@angular/router';
import { Home } from './features/home/home';
import { UpscaleImage } from './features/upscale-image/upscale-image';
import { CanvasCompressImage } from './features/canvas-compress-image/canvas-compress-image';
import { LibcaesiumCompress } from './features/libcaesium-compress/libcaesium-compress';
import { IconManager } from './features/icon-manager/icon-manager';
import { Settings } from './features/settings/settings';

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
    {
        path: 'canvas-compress-image',
        component: CanvasCompressImage,
        title: 'Canvas Compress Image',
    },
    {
        path: 'libcaesium-compress',
        component: LibcaesiumCompress,
        title: 'Libcaesium compress',
    },
    {
        path: 'icon-manager',
        component: IconManager,
        title: 'Icon Manager',
    },
    {
        path: 'settings',
        loadChildren: () =>
            import('./features/settings/settings.routes').then((r) => r.SETTINGS_ROUTES),
    },
];
