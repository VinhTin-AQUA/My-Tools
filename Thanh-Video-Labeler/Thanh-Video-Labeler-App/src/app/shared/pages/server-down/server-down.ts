import { Component } from '@angular/core';

@Component({
    selector: 'app-server-down',
    imports: [],
    templateUrl: './server-down.html',
    styleUrl: './server-down.scss',
})
export class ServerDown {
    reload() {
        window.location.reload();
    }
}
