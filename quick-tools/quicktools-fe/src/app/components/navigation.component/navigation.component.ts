import { Component } from '@angular/core';
import { NavigationService } from '../../services/navigation-service';
import { ButtonModule } from '@openng/optimus-ui/button';

@Component({
    selector: 'app-navigation-component',
    imports: [ButtonModule],
    templateUrl: './navigation.component.html',
    styleUrl: './navigation.component.css',
})
export class NavigationComponent {
    constructor(private readonly navigationService: NavigationService) {}

    back(): void {
        this.navigationService.back();
    }
}
