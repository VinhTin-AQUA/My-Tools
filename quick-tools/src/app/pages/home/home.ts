import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { MAIN_ROUTE, MainRoutes } from '../../core/enums/routes.enum';
import { TauriCommandService } from '../../core/services/tauri-command-service';
import { TranslatePipe } from '@ngx-translate/core';
import { IpStore } from '../../shared/stores/ip.store';

interface Tool {
    id: number;
    name: string;
    icon: string;
    route: string;
}

@Component({
    selector: 'app-home',
    imports: [RouterLink, TranslatePipe],
    templateUrl: './home.html',
    styleUrl: './home.scss',
})
export class Home {
    tools: Tool[] = [
        {
            id: 1,
            name: 'Upscale IMG',
            icon: '🖼️',
            route: `${MAIN_ROUTE}/${MainRoutes.IloveimgUpscaleImage}`,
        },
        {
            id: 2,
            name: 'Split IMG',
            icon: '🖼️',
            route: `${MAIN_ROUTE}/${MainRoutes.SplitImg}`,
        },
    ];

    ipStore = inject(IpStore);

    constructor(private router: Router) {}

    async ngOnInit() {}

    navigateToTool(route: string) {
        this.router.navigateByUrl(route);
    }
}
