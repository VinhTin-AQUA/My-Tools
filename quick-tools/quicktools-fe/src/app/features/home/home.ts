import { Component, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InfoItem, MenuItem } from './models';
import { InputTextModule } from '@openng/optimus-ui/inputtext';
import { ButtonModule } from '@openng/optimus-ui/button';
import { RouterLink } from '@angular/router';
import { IpService } from '../../services/ip-service';
import { IpApiResponse } from '../../models/ip.model';
import { TooltipModule } from '@openng/optimus-ui/tooltip';

@Component({
    selector: 'app-home',
    imports: [CommonModule, FormsModule, InputTextModule, ButtonModule, RouterLink, TooltipModule],
    templateUrl: './home.html',
    styleUrl: './home.css',
})
export class Home {
    infoItems = signal<InfoItem[]>([
        {
            icon: '🌐',
            label: 'Public IP',
            key: 'publicIp',
            value: '-',
        },
        {
            icon: '📍',
            label: 'Autonomous System',
            key: 'as',
            value: '-',
        },
        {
            icon: '🖥️',
            label: 'ISP',
            key: 'isp',
            value: '-',
        },
        {
            icon: '🏙️',
            label: 'City',
            key: 'city',
            value: '-',
        },
    ]);

    menuItems: MenuItem[] = [
        { icon: '🖼️', name: 'Upscale Img', route: '/upscale-image' },
        { icon: '🖼️', name: 'Split Img', route: '/profile' },
        { icon: '🖼️', name: 'Canvas Compress Image', route: '/canvas-compress-image' },
        { icon: '⬇', name: 'Libcaesium Compress', route: '/libcaesium-compress' },
        { icon: '😀', name: 'Icon Manager', route: '/icon-manager' },
    ];

    searchTerm: string = '';

    get filteredMenuItems(): MenuItem[] {
        if (!this.searchTerm.trim()) {
            return this.menuItems;
        }
        return this.menuItems.filter((item) =>
            item.name.toLowerCase().includes(this.searchTerm.toLowerCase()),
        );
    }

    constructor(private ipService: IpService) {}

    async ngOnInit() {
        this.getIpInformation();
    }

    getIpInformation(): void {
        this.ipService.getIpInformation().subscribe({
            next: (result: IpApiResponse) => {
                console.log(result);

                const values: Record<string, string> = {
                    publicIp: result.query || '-',
                    as: `${result.as}`,
                    isp: result.isp || '-',
                    city: result.city || '-',
                };

                this.infoItems.update((items) =>
                    items.map((item) => ({
                        ...item,
                        value: values[item.key] ?? item.value,
                    })),
                );
            },

            error: (error) => {
                console.error('Failed to get IP information:', error);
            },
        });
    }
}
