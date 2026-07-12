import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { NavItem } from './models/nav-item';
import { filter } from 'rxjs';
import { LayoutRoutes, MainRoutes } from '../../../core/constants/routes-consts';

@Component({
    selector: 'app-main-layout',
    imports: [RouterOutlet, CommonModule, TranslatePipe, RouterLink, RouterLinkActive],
    templateUrl: './main-layout.html',
    styleUrl: './main-layout.css',
})
export class MainLayout {
    isDrawerOpen = false;
    navItems: NavItem[] = [];

    constructor(private router: Router) {}

    ngOnInit() {
        this.navItems = [
            {
                name: 'menuSidebar.home',
                url: `/${MainRoutes.HOME}`,
            },
            {
                name: 'menuSidebar.startExam',
                url: `/${MainRoutes.START_EXAM}`,
            },
            {
                name: 'menuSidebar.questionBank',
                url: `/${MainRoutes.QUESTION_BANK}`,
            },
            {
                name: 'menuSidebar.settings',
                url: `/${MainRoutes.SETTINGS}`,
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
