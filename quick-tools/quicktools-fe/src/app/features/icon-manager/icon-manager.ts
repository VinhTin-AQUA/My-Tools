import { Component, inject, signal, ViewChild } from '@angular/core';
import { DeleteIconRequest, IconModel, SearchIconRequest, SearchIconResponse } from './icon.models';
import { ButtonModule } from '@openng/optimus-ui/button';
import { FormsModule } from '@angular/forms';
import { InputTextModule } from '@openng/optimus-ui/inputtext';
import { WebuiService } from '../../services/webui-service';
import { MenuItem, MessageService } from '@openng/optimus-ui/api';
import { AddIcon } from './components/add-icon/add-icon';
import { DialogModule } from '@openng/optimus-ui/dialog';
import { InputGroupModule } from '@openng/optimus-ui/inputgroup';
import { InputGroupAddonModule } from '@openng/optimus-ui/inputgroupaddon';
import { TagModule } from '@openng/optimus-ui/tag';
import { PaginatorModule } from '@openng/optimus-ui/paginator';
import { Menu, MenuModule } from '@openng/optimus-ui/menu';
import { TooltipModule } from '@openng/optimus-ui/tooltip';
import { ToastModule } from '@openng/optimus-ui/toast';
import { AddMultiIcons } from './components/add-multi-icons/add-multi-icons';
import { NavigationComponent } from "../../components/navigation.component/navigation.component";

@Component({
    selector: 'app-icon-manager',
    imports: [
    ButtonModule,
    FormsModule,
    InputTextModule,
    AddIcon,
    DialogModule,
    InputGroupModule,
    InputGroupAddonModule,
    TagModule,
    PaginatorModule,
    MenuModule,
    TooltipModule,
    ToastModule,
    AddMultiIcons,
    NavigationComponent
],
    templateUrl: './icon-manager.html',
    styleUrl: './icon-manager.css',
    providers: [MessageService],
})
export class IconManager {
    readonly searchTerm = signal('');
    showAddDialog = signal(false);
    showAddMultiIconsDialog = signal(false);
    selectedIcon = signal<IconModel | null>(null);
    page = signal<number>(1);
    pageSize = signal<number>(20);

    icons = signal<IconModel[]>([
        // {
        //     id: '',
        //     name: 'Funny Dance',
        //     iconType: 0,
        //     url: 'https://media4.giphy.com/media/v1.Y2lkPTE5NGEwMzQ5dXJ5YnU4b2FqencyZjJ4OXlvbXVzcGpjMjE0eWx2MGJ6Y2F0eW9yZSZlcD12MV9naWZzX2dpZklkJmN0PXM/9Ztp68jWLQE5DrJAZM/200.gif',
        // },
    ]);

    readonly menuIcon = signal<IconModel | null>(null);

    readonly iconMenuItems: MenuItem[] = [
        {
            label: 'Copy link',
            icon: 'pi pi-copy',
            command: () => {
                const icon = this.menuIcon();
                if (icon) {
                    this.copyIconLink(icon);
                }
            },
        },
        {
            label: 'Open link',
            icon: 'pi pi-external-link',
            command: () => {
                const icon = this.menuIcon();
                if (icon) {
                    this.openIconLink(icon);
                }
            },
        },
        {
            separator: true,
        },
        {
            label: 'Download',
            icon: 'pi pi-download',
            command: () => {
                const icon = this.menuIcon();
                if (icon) {
                    this.downloadIcon(icon);
                }
            },
        },
        {
            label: 'Delete',
            icon: 'pi pi-trash',
            command: () => {
                const icon = this.menuIcon();
                if (icon) {
                    this.deleteIcon(icon);
                }
            },
        },
    ];

    private messageService = inject(MessageService);

    @ViewChild('iconMenu') iconMenu!: Menu;

    constructor(private webuiService: WebuiService) {}

    async ngOnInit() {
        await this.searchicons();
    }

    /* ========================= api actions ========================= */

    async searchicons() {
        try {
            const request: SearchIconRequest = {
                keyword: this.searchTerm(),
                page: this.page(),
                pageSize: this.pageSize(),
            };

            console.log(request);

            const r = await this.webuiService.callJson<SearchIconResponse>('searchIcons', request);

            console.log(r);

            if (r) {
                this.icons.set(r.items);
                console.log(this.icons());
            } else {
                this.messageService.add({
                    severity: 'error',
                    summary: 'Get icons failed',
                    detail: '',
                });
            }
        } catch (ex) {
            console.log(ex);

            this.messageService.add({
                severity: 'error',
                summary: 'Get icons failed',
                detail: '',
            });
        }
    }

    async deleteIcon(icon: IconModel) {
        const deleteIconRequest: DeleteIconRequest = {
            id: icon.id,
        };
        const r = await this.webuiService.callJson<boolean>('deleteIcon', deleteIconRequest);
        console.log(r);

        if (r) {
            this.messageService.add({
                severity: 'success',
                summary: 'Delete icon successfully',
                detail: icon.name,
            });

            await this.searchicons();
        } else {
            this.messageService.add({
                severity: 'error',
                summary: 'Delete icon failed',
                detail: icon.name,
            });
        }
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

    /* ========================= ui actions ========================= */
    openAddDialog(): void {
        this.showAddDialog.set(true);
    }

    async closeAddDialog() {
        this.showAddDialog.set(false);
        await this.searchicons();
    }

    openAddMultiIconDialog(): void {
        this.showAddMultiIconsDialog.set(true);
    }

    async closeAddMultiIconDialog() {
        this.showAddMultiIconsDialog.set(false);
        await this.searchicons();
    }

    async onPageChange() {
        this.page.update((x) => x + 1);
        await this.searchicons();
    }

    openIconMenu(event: Event, icon: IconModel): void {
        this.menuIcon.set(icon);
        this.iconMenu.toggle(event);
    }

    async copyIconLink(icon: IconModel): Promise<void> {
        try {
            await navigator.clipboard.writeText(icon.url);

            this.messageService.add({
                severity: 'success',
                summary: 'Link copied',
                detail: icon.name,
            });
        } catch (error) {
            console.error('Unable to copy icon URL:', error);

            this.messageService.add({
                severity: 'error',
                summary: 'Copy failed',
                detail: icon.name,
            });
        }
    }

    openIconLink(icon: IconModel): void {
        window.open(icon.url, '_blank', 'noopener,noreferrer');
    }
}
