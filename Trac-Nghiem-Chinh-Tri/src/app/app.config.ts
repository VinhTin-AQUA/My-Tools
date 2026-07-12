import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, withInMemoryScrolling, withViewTransitions } from '@angular/router';
import { provideTranslateService } from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
    providers: [
        provideBrowserGlobalErrorListeners(),
        provideRouter(
            routes,
            withViewTransitions(),
            withInMemoryScrolling({
                scrollPositionRestoration: 'top',
                anchorScrolling: 'enabled',
            })
        ),
        provideTranslateService({
            loader: provideTranslateHttpLoader({
                prefix: '/i18n/',
                suffix: '.json',
            }),
            fallbackLang: 'vi',
            lang: 'vi',
        }),
    ],
};
