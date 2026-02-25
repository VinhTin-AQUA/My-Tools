import { Component, inject, signal } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { Loader } from './shared/components/loader/loader';
import { QuestionCancelDialog } from './shared/components/question-cancel-dialog/question-cancel-dialog';
import { Toast } from './shared/components/toast/toast';
import { DialogService } from './core/services/dialog-service';
import { LanguageService } from './core/services/language-service';
import { TauriCommandService } from './core/services/tauri-command-service';
import { Commands } from './core/enums/commands.enum';
import { SplashScreen } from './shared/components/splash-screen/splash-screen';
import { IpAddress, IpAddressInfo } from './core/models/payload-commands/ip.payload';
import { IpStore } from './shared/stores/ip.store';

@Component({
    selector: 'app-root',
    imports: [RouterOutlet, Loader, QuestionCancelDialog, Toast, SplashScreen],
    templateUrl: './app.html',
    styleUrl: './app.scss',
})
export class App {
    protected readonly title = signal('quick-tools');
    success = signal<boolean>(false);
    ipStore = inject(IpStore);

    constructor(
        public dialogService: DialogService,
        private languageService: LanguageService,
        private tauriCommandService: TauriCommandService
    ) {}

    async ngOnInit() {
        const success = await this.tauriCommandService.invokeCommand<boolean>(
            Commands.INIT_COMPLETE,
            {},
            false
        );

        const ipAddress = await this.tauriCommandService.invokeCommand<IpAddress>(
            Commands.GET_YOUR_IP_COMMAND,
            {},
        );

        if (!ipAddress) {
            return;
        }

        const ipAddressInfo = await this.tauriCommandService.invokeCommand<IpAddressInfo>(
            Commands.GET_YOUR_IP_INFO_COMMAND,
            ipAddress,
        );

        if (!ipAddressInfo) {
            return;
        }

        this.ipStore.update(ipAddressInfo);

        if (success) {
            this.success.set(success);
        } else {
            this.success.set(false);
        }
    }
}
