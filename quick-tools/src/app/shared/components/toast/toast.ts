import { Component, inject } from '@angular/core';
import { ToastMessageStore } from '../../stores/toast.store';
import { CommonModule } from '@angular/common';
import { DialogService } from '../../../core/services/dialog-service';

@Component({
  selector: 'app-toast',
  imports: [CommonModule],
  templateUrl: './toast.html',
  styleUrl: './toast.scss',
})
export class Toast {
toastMessageStore = inject(ToastMessageStore);

	constructor(private dialogService: DialogService) {}

	hide() {
		this.dialogService.showToastMessage(false, '', '', true);
	}
}
