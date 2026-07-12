import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LanguageService } from './core/services/language-service';
import { Loader } from './shared/components/loader/loader';
import { DialogService } from './core/services/dialog-service';
import { ToastStore } from './shared/stores/toast.store';
import { Toast } from './shared/components/toast/toast';

@Component({
    selector: 'app-root',
    imports: [RouterOutlet, Loader, Toast],
    templateUrl: './app.html',
    styleUrl: './app.css',
})
export class App {
    protected readonly title = signal('Trac-Nghiem-Chinh-Tri');
    toastStore = ToastStore;

    constructor(private languageService: LanguageService, public dialogService: DialogService) {}
}
