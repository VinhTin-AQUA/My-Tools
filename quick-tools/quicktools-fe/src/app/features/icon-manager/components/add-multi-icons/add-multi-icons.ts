import { Component, inject, output } from '@angular/core';
import { ButtonModule } from '@openng/optimus-ui/button';
import { FormsModule } from '@angular/forms';
import { TextareaModule } from '@openng/optimus-ui/textarea';
import { WebuiService } from '../../../../services/webui-service';
import { MessageService } from '@openng/optimus-ui/api';
import { ToastModule } from '@openng/optimus-ui/toast';
import { AddIconRequest, IconType } from '../../icon.models';
import { RadioButtonModule } from '@openng/optimus-ui/radiobutton';

@Component({
    selector: 'app-add-multi-icons',
    imports: [ButtonModule, FormsModule, TextareaModule, ToastModule, RadioButtonModule],
    templateUrl: './add-multi-icons.html',
    styleUrl: './add-multi-icons.css',
    providers: [MessageService],
})
export class AddMultiIcons {
    inputText = '';
    selectedIconType: IconType = IconType.Gift;

    protected readonly IconType = IconType;
    protected readonly iconTypes = [
        {
            value: IconType.Gift,
            label: 'Gift',
            description: 'Use a gift icon',
            icon: 'pi pi-gift',
        },
        {
            value: IconType.Image,
            label: 'Image',
            description: 'Use an image icon',
            icon: 'pi pi-image',
        },
    ];

    private messageService = inject(MessageService);

    constructor(private webuiService: WebuiService) {}

    async submit() {
        const lines = this.inputText
            .split(/\r?\n/)
            .map((line) => line.trim())
            .filter(Boolean);

        const items: AddIconRequest[] = [];

        for (let i = 0; i < lines.length; i += 2) {
            const name = lines[i];
            const url = lines[i + 1];

            if (!name || !url) {
                continue;
            }

            items.push({
                name,
                url,
                iconType: this.selectedIconType,
            });
        }

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
