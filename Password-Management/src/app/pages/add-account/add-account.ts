import { Component, inject } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { TauriCommandSerivce } from '../../shared/services/tauri-command-service';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ResponseCommand } from '../../core/models/response-command';
import { SpreadsheetConfigStore } from '../../shared/stores/spread-sheet-config-store';
import { Router } from '@angular/router';
import { AUTH_ROUTE, AuthRoutes } from '../../core/enums/routes.enum';
import { AccountModel } from '../../core/models/account-model';
import { DialogService } from '../../shared/services/dialog-service';
import { TextInput } from '../../shared/components/text-input/text-input';
import { PasswordInput } from '../../shared/components/password-input/password-input';
import { TextAreaInput } from '../../shared/components/text-area-input/text-area-input';
import { AppStore } from '../../shared/stores/app.store';
import { AccountStore } from '../../shared/stores/accounts.store';

@Component({
    selector: 'app-add-account',
    imports: [TranslatePipe, ReactiveFormsModule, TextInput, PasswordInput, TextAreaInput],
    templateUrl: './add-account.html',
    styleUrl: './add-account.scss',
})
export class AddAccount {
    showPassword = false;
    submitted: boolean = false;
    form!: FormGroup;
    spreadsheetConfigStore = inject(SpreadsheetConfigStore);
    appStore = inject(AppStore);
    accounts = inject(AccountStore);

    constructor(
        private tauriCommandSerivce: TauriCommandSerivce,
        private fb: FormBuilder,
        private router: Router,
        private dialogService: DialogService
    ) {}

    ngOnInit() {
        this.form = this.fb.group({
            accountName: ['', Validators.required],
            userName: ['', Validators.required],
            password: ['', Validators.required],
            confirmPassword: ['', Validators.required],
            note: [''],
        });
        this.init();
    }

    async save() {
        this.submitted = true;
        if (this.form.invalid) {
            return;
        }

        if (this.form.controls['password'].value !== this.form.controls['confirmPassword'].value) {
            return;
        }

        // TODO: Gửi dữ liệu lên backend hoặc emit event
        const addAccount: AccountModel = {
            id: crypto.randomUUID(),
            account_name: this.form.controls['accountName'].value,
            note: this.form.controls['note'].value,
            password: this.form.controls['password'].value,
            user_name: this.form.controls['userName'].value,
            salt_base64: '',
        };

        const response = await this.tauriCommandSerivce.invokeCommand<ResponseCommand>(
            TauriCommandSerivce.ADD_ACCOUNT,
            {
                passcode: this.appStore.passCode(),
                password: addAccount,
            }
        );

        if (response?.is_success) {
            this.accounts.addAccount(addAccount);
            this.dialogService.showToastMessage(true, 'Success', 'Add account successfully', true);
        } else {
            this.dialogService.showToastMessage(true, 'Failed', 'Something error', false);
        }
    }

    private async init() {
        if (!this.appStore.passCode()) {
            this.router.navigateByUrl(`/${AUTH_ROUTE}/${AuthRoutes.AddPasscode}`);
        }
    }
}
