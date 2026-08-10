import { Component, signal } from '@angular/core';
import { WebuiService } from './services/webui-service';
import { RouterOutlet } from '@angular/router';

@Component({
    selector: 'app-root',
    imports: [RouterOutlet],
    templateUrl: './app.html',
    styleUrl: './app.css',
})
export class App {
    protected readonly title = signal('quicktools-fe');

    constructor(private webuiService: WebuiService) {}

    ngOnInit() {}

    //

    async longTask() {
        const r = await this.webuiService.call<string>('longTask', 2);
        console.log(r);
    }

    async getData() {
        const r = await this.webuiService.call<any>('getData');
        console.log(r);
    }

    async sendData() {
        const r = await this.webuiService.callJson<string>('sendData', {
            name: 'Pootin',
            age: 10,
        });
        console.log(r);
    }

    async asyncFunction() {
        const r = await this.webuiService.call<string>('asyncFunction');
        console.log(r);
    }
}
