import { Directive, DOCUMENT, inject, input, OnDestroy, OnInit } from '@angular/core';

@Directive({
    selector: '[appBlockOutsideClose]',
})
export class BlockOutsideClose implements OnInit, OnDestroy {
    readonly appBlockOutsideClose = input(false);

    private readonly document = inject(DOCUMENT);

    private readonly onMouseDown = (event: MouseEvent): void => {
        if (!this.appBlockOutsideClose()) {
            return;
        }

        const target = event.target as HTMLElement | null;

        if (target?.classList.contains('p-dialog-mask')) {
            event.stopPropagation();
            event.preventDefault();
        }
    };

    ngOnInit(): void {
        this.document.addEventListener('mousedown', this.onMouseDown, true);
    }

    ngOnDestroy(): void {
        this.document.removeEventListener('mousedown', this.onMouseDown, true);
    }
}
