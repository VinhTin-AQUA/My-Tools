import { Component } from '@angular/core';
import { Themes } from '../../components/themes/themes';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-appearance',
    imports: [CommonModule, Themes],
    templateUrl: './appearance.html',
    styleUrl: './appearance.css',
})
export class Appearance {}
