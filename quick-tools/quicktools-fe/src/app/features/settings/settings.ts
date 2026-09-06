import { Component } from '@angular/core';
import { SETTINGS_GROUPS, SettingsGroup } from './settings.routes';
import { NavigationEnd, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NavigationComponent } from "../../components/navigation.component/navigation.component";

@Component({
    selector: 'app-settings',
    imports: [CommonModule, RouterModule, NavigationComponent],
    templateUrl: './settings.html',
    styleUrl: './settings.css',
})
export class Settings {
    settingsGroups = SETTINGS_GROUPS;
    activeGroupId: string = 'general';

    constructor(private router: Router) {}

    ngOnInit() {
        this.updateActiveGroup(this.router.url);

        this.router.events.subscribe((event) => {
            if (event instanceof NavigationEnd) {
                this.updateActiveGroup(event.url);
            }
        });
    }

    updateActiveGroup(url: string) {
        const group = this.settingsGroups.find((g) => url.includes(g.route));
        if (group) {
            this.activeGroupId = group.id;
        }
    }

    isActive(group: SettingsGroup): boolean {
        return this.activeGroupId === group.id;
    }

    navigateTo(route: string) {
        this.router.navigate([route]);
    }
}
