import { Component, signal, viewChild, ViewChild } from '@angular/core';
import { MenuItem, MessageService } from '@openng/optimus-ui/api';
import { ButtonModule } from '@openng/optimus-ui/button';
import { Menu, MenuModule } from '@openng/optimus-ui/menu';
import { ToastModule } from '@openng/optimus-ui/toast';
import { TooltipModule } from '@openng/optimus-ui/tooltip';
import { Loader } from '../../components/loader/loader';
import { NavigationComponent } from '../../components/navigation.component/navigation.component';
import { WebUIResponse } from '../../models/webui.response';
import { WebuiService } from '../../services/webui-service';
import { PasswordManagerAddPassword } from './components/password-manager-add-password/password-manager-add-password';
import { PasswordManagerDeletePassword } from './components/password-manager-delete-password/password-manager-delete-password';
import { PasswordManagerEditPassword } from './components/password-manager-edit-password/password-manager-edit-password';
import { PasswordManagerSetKey } from './components/password-manager-set-key/password-manager-set-key';
import { PasswordManagerSetUrl } from './components/password-manager-set-url/password-manager-set-url';
import { PasswordManagerViewPassword } from './components/password-manager-view-password/password-manager-view-password';
import { PasswordManagerModel } from './models/password-manager.model';
import { GoogleSheetApiResult } from '../../models/google-sheet-api.model';

@Component({
    selector: 'app-password-manager',
    imports: [
        ButtonModule,
        MenuModule,
        ToastModule,
        TooltipModule,
        NavigationComponent,
        Loader,
        PasswordManagerAddPassword,
        PasswordManagerDeletePassword,
        PasswordManagerEditPassword,
        PasswordManagerSetKey,
        PasswordManagerSetUrl,
        PasswordManagerViewPassword,
    ],
    templateUrl: './password-manager.html',
    styleUrl: './password-manager.css',
    providers: [MessageService],
})
export class PasswordManager {
    readonly passwords = signal<PasswordManagerModel[]>([]);
    readonly selectedPassword = signal<PasswordManagerModel | null>(null);
    readonly isLoading = signal(false);
    readonly isBackingUp = signal(false);

    readonly key = signal('');
    readonly url = signal('');

    readonly showSetKey = signal(false);
    readonly showSetUrl = signal(false);
    readonly showViewPassword = signal(false);
    readonly showEditPassword = signal(false);
    readonly showDeletePassword = signal(false);
    readonly showAddPassword = signal(false);

    readonly headerMenuItems: MenuItem[] = [
        {
            label: 'Set Key',
            icon: 'pi pi-key',
            command: () => this.onSetKey(),
        },
        {
            label: 'Set URL',
            icon: 'pi pi-link',
            command: () => this.onSetUrl(),
        },
        {
            label: 'Backup',
            icon: 'pi pi-file-export',
            command: () => this.duplicateFirstSheet(),
        },
    ];

    readonly itemMenuItems: MenuItem[] = [
        {
            label: 'View Password',
            icon: 'pi pi-eye',
            command: () => this.onViewPassword(),
        },
        {
            label: 'Edit',
            icon: 'pi pi-pencil',
            command: () => this.onEditPassword(),
        },
        {
            label: 'Delete',
            icon: 'pi pi-trash',
            command: () => this.onDeletePassword(),
        },
    ];

    // @ViewChild('headerMenu') private headerMenu!: Menu;
    // @ViewChild('itemMenu') private itemMenu!: Menu;

    readonly headerMenu = viewChild<Menu>('headerMenu');
    readonly itemMenu = viewChild<Menu>('itemMenu');

    constructor(
        private webuiService: WebuiService,
        private messageService: MessageService,
    ) {}

    async ngOnInit() {
        await this.getAllPasswords();
    }

    async getAllPasswords(): Promise<PasswordManagerModel[]> {
        this.isLoading.set(true);

        try {
            const response = await this.webuiService.callJson<
                WebUIResponse<PasswordManagerModel[]>
            >('GoogleSheetPasswordManagerHandler_GetAll', {});

            if (!response.success) {
                this.messageService.add({
                    severity: 'error',
                    summary: response.title || 'Passwords could not be loaded',
                    detail: response.description || 'Unable to load saved passwords.',
                });

                return this.passwords();
            }

            const passwords = response.data ?? [];

            this.passwords.set(passwords);

            return passwords;
        } finally {
            this.isLoading.set(false);
        }
    }

    /* ========================= header actions ========================= */

    async onReload() {
        await this.getAllPasswords();
    }

    openHeaderMenu(event: Event): void {
        this.itemMenu()?.hide();
        this.headerMenu()?.toggle(event);
    }

    onSetKey(): void {
        this.headerMenu()?.hide();
        this.showSetKey.set(true);
    }

    onSetUrl(): void {
        this.headerMenu()?.hide();
        this.showSetUrl.set(true);
    }

    async duplicateFirstSheet() {
        if (this.isBackingUp()) {
            return;
        }

        this.headerMenu()?.hide();
        this.isBackingUp.set(true);

        try {
            const result = await this.webuiService.callJson<WebUIResponse<GoogleSheetApiResult>>(
                'GoogleSheetPasswordManagerHandler_DuplicateFirstSheet',
                {},
            );

            if (!result.success || !result.data) {
                this.messageService.add({
                    severity: 'error',
                    summary: result.title,
                    detail: result.description,
                });

                return;
            }

            this.messageService.add({
                severity: 'success',
                summary: result.title,
                detail: result.description,
            });
        } finally {
            this.isBackingUp.set(false);
        }
    }

    /* ========================= item actions ========================= */

    openItemMenu(event: Event, password: PasswordManagerModel): void {
        this.selectedPassword.set(password);
        this.headerMenu()?.hide();
        this.itemMenu()?.toggle(event);
    }

    onViewPassword(): void {
        if (!this.selectedPassword()) {
            return;
        }

        this.itemMenu()?.hide();
        this.showViewPassword.set(true);
    }

    onEditPassword(): void {
        if (!this.selectedPassword()) {
            return;
        }

        this.itemMenu()?.hide();
        this.showEditPassword.set(true);
    }

    onDeletePassword(): void {
        if (!this.selectedPassword()) {
            return;
        }

        this.itemMenu()?.hide();
        this.showDeletePassword.set(true);
    }

    openAddPassword(): void {
        this.headerMenu()?.hide();
        this.itemMenu()?.hide();
        this.showAddPassword.set(true);
    }

    /* ========================= popup results ========================= */

    onKeySaved(key: string): void {
        this.key.set(key);
    }

    onUrlSaved(url: string): void {
        this.url.set(url);
    }

    onPasswordAdded(password: PasswordManagerModel): void {
        const nextId =
            this.passwords().reduce((highestId, item) => Math.max(highestId, item.id), 0) + 1;

        this.passwords.update((passwords) => [...passwords, { ...password, id: nextId }]);
    }

    onPasswordUpdated(password: PasswordManagerModel): void {
        this.passwords.update((passwords) =>
            passwords.map((item) => (item.id === password.id ? password : item)),
        );

        if (this.selectedPassword()?.id === password.id) {
            this.selectedPassword.set(password);
        }
    }

    onPasswordDeleted(id: number): void {
        this.passwords.update((passwords) => passwords.filter((item) => item.id !== id));

        if (this.selectedPassword()?.id === id) {
            this.selectedPassword.set(null);
        }
    }
}
