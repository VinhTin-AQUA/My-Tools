import { Component, computed, signal } from '@angular/core';
import { IconModel } from './icon.models';
import { ButtonModule } from '@openng/optimus-ui/button';
import { FormsModule } from '@angular/forms';
import { InputTextModule } from '@openng/optimus-ui/inputtext';

@Component({
    selector: 'app-icon-manager',
    imports: [ButtonModule, FormsModule, InputTextModule],
    templateUrl: './icon-manager.html',
    styleUrl: './icon-manager.css',
})
export class IconManager {
    readonly searchTerm = signal('');

    readonly icons = signal<IconModel[]>([
        {
            name: 'Funny Dance',
            iconType: 'image',
            url: 'https://media4.giphy.com/media/v1.Y2lkPTE5NGEwMzQ5dXJ5YnU4b2FqencyZjJ4OXlvbXVzcGpjMjE0eWx2MGJ6Y2F0eW9yZSZlcD12MV9naWZzX2dpZklkJmN0PXM/9Ztp68jWLQE5DrJAZM/200.gif',
        },
        {
            name: 'Cute Reaction',
            iconType: 'image',
            url: 'https://media0.giphy.com/media/v1.Y2lkPTE5NGEwMzQ5OW00OWkyMXozN3V3dW15bWwzc2dxOWRiaW16N2ZpczlnOHN3OGY1dSZlcD12MV9naWZzX2dpZklkJmN0PXM/jY06F91gInHQHGvYbc/200.gif',
        },
        {
            name: 'Happy',
            iconType: 'image',
            url: 'https://media2.giphy.com/media/v1.Y2lkPTE5NGEwMzQ5OHo1enlvdmN1a2VjazlmZnBtbjlqdnpmM2gxaTJndDFjOGR6ejEwYiZlcD12MV9naWZzX2dpZklkJmN0PXM/6Ybo61KCsfGs8/200.gif',
        },
        {
            name: 'Excited',
            iconType: 'image',
            url: 'https://media0.giphy.com/media/v1.Y2lkPTE5NGEwMzQ5Z3I3YWlhemZiMDdhcnFleWRjdnI2M3d1NG4xbDQzYWozbnFhZDg3aiZlcD12MV9naWZzX2dpZklkJmN0PXM/xAd5ir9Gwo26kxbanR/200.gif',
        },
        {
            name: 'Laugh',
            iconType: 'image',
            url: 'https://media0.giphy.com/media/v1.Y2lkPTE5NGEwMzQ5Zmw5ZjI3YzVtbjlhdzBndDVjajZzdnFobjEya3g3eGhmNm9xbGR1cyZlcD12MV9naWZzX2dpZklkJmN0PXM/7CouuG2nJ4jL54tAXA/200.gif',
        },
        {
            name: 'Love',
            iconType: 'image',
            url: 'https://media3.giphy.com/media/v1.Y2lkPTE5NGEwMzQ5a2prZHMxcTM1ZTE2ZXRkcG1pdXhyeDl2YmRlYXczMXc2ZzB1OHJnZyZlcD12MV9naWZzX2dpZklkJmN0PXM/nNFRVw2ZfHQyCec7ed/200.gif',
        },
    ]);

    readonly filteredIcons = computed(() => {
        const keyword = this.searchTerm().trim().toLowerCase();

        if (!keyword) {
            return this.icons();
        }

        return this.icons().filter((icon) => icon.name.toLowerCase().includes(keyword));
    });

    onSearch(value: string): void {
        this.searchTerm.set(value);
    }

    async downloadIcon(icon: IconModel): Promise<void> {
        try {
            const response = await fetch(icon.url);

            if (!response.ok) {
                throw new Error(`Download failed: ${response.status}`);
            }

            const blob = await response.blob();
            const blobUrl = URL.createObjectURL(blob);

            const anchor = document.createElement('a');

            anchor.href = blobUrl;
            anchor.download = `${icon.name}.gif`;

            document.body.appendChild(anchor);
            anchor.click();
            anchor.remove();

            URL.revokeObjectURL(blobUrl);
        } catch (error) {
            console.error('Unable to download icon:', error);
        }
    }
}
