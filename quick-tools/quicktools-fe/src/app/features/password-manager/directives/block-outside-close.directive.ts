import { Directive, DOCUMENT, inject, input, OnDestroy, OnInit, output } from '@angular/core';

@Directive({
    selector: '[appBlockOutsideClose]',
})
export class BlockOutsideClose implements OnInit, OnDestroy {
    readonly appBlockOutsideClose = input(false);
    readonly appBlockOutsideCloseVisible = input(false);
    readonly appBlockOutsideCloseDismiss = output<void>();

    private readonly document = inject(DOCUMENT);

    // PrimeNG binds its mask listener when the dialog opens, so the outside click
    // is handled here instead of relying on dismissableMask toggling.
    private readonly onMouseDown = (event: MouseEvent): void => {
        const target = event.target as HTMLElement | null;

        if (
            !target?.classList.contains('p-dialog-mask') ||
            !this.appBlockOutsideCloseVisible()
        ) {
            return;
        }

        event.stopPropagation();
        event.preventDefault();

        if (this.appBlockOutsideClose()) {
            return;
        }

        this.appBlockOutsideCloseDismiss.emit();
    };

    ngOnInit(): void {
        this.document.addEventListener('mousedown', this.onMouseDown, true);
    }

    ngOnDestroy(): void {
        this.document.removeEventListener('mousedown', this.onMouseDown, true);
    }
}
