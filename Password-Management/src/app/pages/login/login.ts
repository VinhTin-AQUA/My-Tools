import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { AUTH_ROUTE, AuthRoutes, MAIN_ROUTE, MainRoutes } from '../../core/enums/routes.enum';
import { Icon } from '../../shared/components/icon/icon';
import { PasswordInput } from '../../shared/components/password-input/password-input';
import { AppStore } from '../../shared/stores/app.store';

@Component({
    selector: 'app-login',
    imports: [FormsModule, TranslatePipe, Icon, PasswordInput],
    templateUrl: './login.html',
    styleUrl: './login.scss',
})
export class Login {
    password = '';
    errorMessage: string = '';
    showPassword: boolean = false;
    appStore = inject(AppStore);

    constructor(private router: Router) {}

    ngOnInit() {
        this.Init();
    }

    onLogin() {
        if (this.appStore.passCode() !== this.password) {
            this.errorMessage = 'addPasscodeForm.passcodeIsNotValid';
            return;
        }

        this.router.navigateByUrl(`/${MAIN_ROUTE}/${MainRoutes.Home}`);
    }

    onFingerprint() {
        console.log('Fingerprint login clicked');
    }

    private async Init() {
        if (!this.appStore.passCode()) {
            this.router.navigateByUrl(`/${AUTH_ROUTE}/${AuthRoutes.AddPasscode}`);
        }
    }
}
