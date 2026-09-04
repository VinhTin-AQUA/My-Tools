import { Component, inject, output } from '@angular/core';
import { ButtonModule } from '@openng/optimus-ui/button';
import { FormsModule } from '@angular/forms';
import { TextareaModule } from '@openng/optimus-ui/textarea';
import { WebuiService } from '../../../../services/webui-service';
import { MessageService } from '@openng/optimus-ui/api';
import { ToastModule } from '@openng/optimus-ui/toast';

interface LinkItem {
    name: string;
    url: string;
}

@Component({
    selector: 'app-add-multi-icons',
    imports: [ButtonModule, FormsModule, TextareaModule, ToastModule],
    templateUrl: './add-multi-icons.html',
    styleUrl: './add-multi-icons.css',
    providers: [MessageService],
})
export class AddMultiIcons {
    inputText = '';

    private messageService = inject(MessageService);

    constructor(private webuiService: WebuiService) {}

    async submit() {
        const lines = this.inputText
            .split(/\r?\n/)
            .map((line) => line.trim())
            .filter(Boolean);

        const items: LinkItem[] = [];

        for (let i = 0; i < lines.length; i += 2) {
            const name = lines[i];
            const url = lines[i + 1];

            if (!name || !url) {
                continue;
            }

            items.push({
                name,
                url,
            });
        }

        console.log(items);

        const r = await this.webuiService.callJson('addMultiIcons', items);

        if (r) {
            this.messageService.add({
                severity: 'success',
                summary: 'Add icon successfully',
                detail: r.name,
            });
        } else {
            this.messageService.add({
                severity: 'error',
                summary: 'Add icon failed',
                detail: '',
            });
        }
    }
}
