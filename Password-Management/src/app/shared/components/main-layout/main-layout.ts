import { Component } from '@angular/core';
import { NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { NavItem } from '../../../core/models/nav-item';
import { filter } from 'rxjs';
import { TranslatePipe } from '@ngx-translate/core';
import { Icon } from '../icon/icon';

@Component({
    selector: 'app-main-layout',
    imports: [RouterOutlet, RouterLink, RouterLinkActive, TranslatePipe, Icon],
    templateUrl: './main-layout.html',
    styleUrl: './main-layout.scss',
})
export class MainLayout {
    isDrawerOpen = false;
    navItems: NavItem[] = [];

    constructor(private router: Router) {}

    ngOnInit() {
        this.navItems = [
            {
                name: 'menuSidebar.home',
                url: '/main/home',
            },
            {
                name: 'menuSidebar.settings',
                url: '/main/settings',
            },
        ];

        this.router.events.pipe(filter((event) => event instanceof NavigationEnd)).subscribe(() => {
            if (this.isDrawerOpen) {
                this.toggleDrawer();
            }
        });
    }

    toggleDrawer() {
        this.isDrawerOpen = !this.isDrawerOpen;

        if (this.isDrawerOpen) {
            document.body.classList.add('overflow-hidden');
        } else {
            document.body.classList.remove('overflow-hidden');
        }
    }
}
