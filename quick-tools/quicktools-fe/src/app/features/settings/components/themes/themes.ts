import { Component, inject } from '@angular/core';
import { ThemeService } from '../../../../services/theme-service';
import { FormsModule } from '@angular/forms';
import { Select } from '@openng/optimus-ui/select';
import { LIST_PRESETS, PresetKey } from '../../../../presets/theme-presets';

@Component({
    selector: 'app-themes',
    imports: [FormsModule, Select],
    templateUrl: './themes.html',
    styleUrl: './themes.css',
})
export class Themes {
    private readonly themeService = inject(ThemeService);

    readonly presets = LIST_PRESETS;

    selectedPreset: PresetKey = this.themeService.preset;

    onPresetChange(preset: PresetKey | null): void {
        if (!preset) {
            return;
        }

        this.themeService.setPreset(preset);
        this.selectedPreset = preset;
    }
}
