import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { ClickOutsideDirective } from '../../shared/directives/click-outside.directive';
import { AUTH_ROUTE, AuthRoutes, MAIN_ROUTE, MainRoutes } from '../../core/enums/routes.enum';
import { TauriCommandSerivce } from '../../shared/services/tauri-command-service';
import { SpreadsheetConfigStore } from '../../shared/stores/spread-sheet-config-store';
import { QuestionCancelDialog } from '../../shared/components/question-cancel-dialog/question-cancel-dialog';
import { AccountModel } from '../../core/models/account-model';
import { UpdateAccountStore } from '../../shared/stores/update-account.store';
import { Icon } from '../../shared/components/icon/icon';
import { AccountStore } from '../../shared/stores/accounts.store';
import { DialogService } from '../../shared/services/dialog-service';
import { AppStore } from '../../shared/stores/app.store';

@Component({
    selector: 'app-home',
    imports: [RouterLink, ClickOutsideDirective, QuestionCancelDialog, Icon],
    templateUrl: './home.html',
    styleUrl: './home.scss',
})
export class Home {
    openMenu: number | null = null;
    speedDialOpen = false;
    isShowQuestionDialog: boolean = false;
    idToDelete: string | null = null;
    accountPaddings = [1, 2, 3];

    spreadsheetConfigStore = inject(SpreadsheetConfigStore);
    updateAccountStore = inject(UpdateAccountStore);
    appStore = inject(AppStore);
    accountStore = inject(AccountStore);

    constructor(
        private router: Router,
        private tauriCommandSerivce: TauriCommandSerivce,
        private dialogService: DialogService
    ) {}

    async ngOnInit() {
        await this.getSavedPasscode();
        await this.getAccounts();
    }

    toggleMenu(i: number, event: Event) {
        event.preventDefault();
        event.stopPropagation();
        this.openMenu = this.openMenu === i ? null : i;
    }

    onCopy(acc: AccountModel, event: Event) {
        event.preventDefault();
        event.stopPropagation();
        console.log('Copy', acc);
        navigator.clipboard.writeText(acc.password);
        this.openMenu = null;
    }

    onUpdate(acc: AccountModel) {
        this.updateAccountStore.update(acc);
        this.router.navigateByUrl(`/${MAIN_ROUTE}/${MainRoutes.EditAccount}`);
    }

    async onDelete(acc: AccountModel, event: Event) {
        event.preventDefault();
        event.stopPropagation();
        this.openMenu = null;
        this.isShowQuestionDialog = true;
        this.idToDelete = acc.id;
    }

    async onShowQuestionDialogClose(result: boolean) {
        this.isShowQuestionDialog = false;

        if (!result) {
            this.idToDelete = null;
            return;
        }

        if (!this.idToDelete) {
            return;
        }

        const check = await this.tauriCommandSerivce.invokeCommand<boolean>(
            TauriCommandSerivce.DELETE_ACCOUNT,
            {
                id: this.idToDelete,
            }
        );

        if (check) {
            this.accountStore.delele(this.idToDelete);
            this.dialogService.showToastMessage(
                true,
                'Success',
                'Delete Account Successfully',
                true
            );
        } else {
            this.dialogService.showToastMessage(true, 'Failed', 'Something error', false);
        }
    }

    toggleSpeedDial() {
        this.speedDialOpen = !this.speedDialOpen;
    }

    closeSpeedDial() {
        this.speedDialOpen = false;
    }

    closeContextMenu() {
        this.openMenu = null;
    }

    private async getAccounts() {
        if (this.accountStore.accounts().length > 0) {
            return;
        }

        const r = await this.tauriCommandSerivce.invokeCommand<AccountModel[]>(
            TauriCommandSerivce.GET_ACCOUNTS,
            {
                passcode: this.appStore.passCode(),
            }
        );

        if (r) {
            this.accountStore.setAccounts(r);
        }
    }

    private async getSavedPasscode() {
        if (!this.appStore.passCode()) {
            this.router.navigateByUrl(`/${AUTH_ROUTE}/${AuthRoutes.AddPasscode}`);
        }
    }
}
