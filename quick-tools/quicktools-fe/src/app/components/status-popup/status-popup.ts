import { Component, input, output } from '@angular/core';
import { ButtonModule } from '@openng/optimus-ui/button';
import { DialogModule } from '@openng/optimus-ui/dialog';
import { NavigationComponent } from "../navigation.component/navigation.component";
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-status-popup',
    imports: [DialogModule, ButtonModule, NavigationComponent, CommonModule],
    templateUrl: './status-popup.html',
    styleUrl: './status-popup.css',
})
export class StatusPopup {
    title = input.required<string>();

    message = input.required<string>();

    success = input(false);
}
