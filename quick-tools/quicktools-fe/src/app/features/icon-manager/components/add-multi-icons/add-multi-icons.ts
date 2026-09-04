import { Component, output } from '@angular/core';
import { ButtonModule } from '@openng/optimus-ui/button';
import { FormsModule } from '@angular/forms';
import { TextareaModule } from '@openng/optimus-ui/textarea';

interface LinkItem {
    name: string;
    url: string;
}

@Component({
    selector: 'app-add-multi-icons',
    imports: [ButtonModule, FormsModule, TextareaModule],
    templateUrl: './add-multi-icons.html',
    styleUrl: './add-multi-icons.css',
})
export class AddMultiIcons {
    inputText = '';

    readonly example = `name1
https://example1.com
name2
https://example2.com
name3
https://example3.com
name4
https://example4.com`;

    submit(): void {
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
    }
}
