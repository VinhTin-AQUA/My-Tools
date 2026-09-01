import { Component, computed, EventEmitter, inject, Output, signal } from '@angular/core';
import { ButtonModule } from '@openng/optimus-ui/button';
import { AddIconRequest, IconModel, IconType } from '../../icon.models';
import { InputTextModule } from '@openng/optimus-ui/inputtext';
import { MessageService } from '@openng/optimus-ui/api';
import { WebuiService } from '../../../../services/webui-service';
import { ToastModule } from '@openng/optimus-ui/toast';

@Component({
    selector: 'app-add-icon',
    imports: [ButtonModule, InputTextModule, ToastModule],
    templateUrl: './add-icon.html',
    styleUrl: './add-icon.css',
    providers: [MessageService],
})
export class AddIcon {
    @Output() closePopup = new EventEmitter<void>();

    onClosePopup() {
        this.closePopup.emit();
    }

    protected readonly IconType = IconType;
    protected readonly iconTypes = [IconType.Gift, IconType.Image];

    protected readonly name = signal('');
    protected readonly url = signal('');
    protected readonly iconType = signal(IconType.Gift);

    protected readonly nameTouched = signal(false);
    protected readonly urlTouched = signal(false);

    protected readonly nameValid = computed(() => this.name().trim().length > 0);

    protected readonly urlValid = computed(() => {
        const value = this.url().trim();

        if (!value) {
            return false;
        }

        try {
            const url = new URL(value);

            return url.protocol === 'http:' || url.protocol === 'https:';
        } catch {
            return false;
        }
    });

    protected readonly formValid = computed(() => this.nameValid() && this.urlValid());

    private messageService = inject(MessageService);

    constructor(private webuiService: WebuiService) {}

    protected setName(value: string): void {
        this.name.set(value);
    }

    protected setUrl(value: string): void {
        this.url.set(value);
    }

    protected selectIconType(type: IconType): void {
        this.iconType.set(type);
    }

    protected iconTypeLabel(type: IconType): string {
        return type === IconType.Gift ? 'Gift' : 'Image';
    }

    protected async submit() {
        this.nameTouched.set(true);
        this.urlTouched.set(true);

        if (!this.formValid()) {
            return;
        }

        const request: AddIconRequest = {
            name: this.name().trim(),
            url: this.url().trim(),
            iconType: this.iconType(),
        };

        const r = await this.webuiService.callJson<any>('addIcon', request);

        if (r) {
            this.messageService.add({
                severity: 'success',
                summary: 'Add icon successfully',
                detail: r.name,
            });
        } else {
            this.messageService.add({
                severity: 'success',
                summary: 'Add icon failed',
                detail: this.name().trim(),
            });
        }
    }
}
