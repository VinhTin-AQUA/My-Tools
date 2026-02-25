import { inject, Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { LanguageOption } from '../../core/models/language-option';

@Injectable({
    providedIn: 'root',
})
export class LanguageService {
    public static readonly LANGUAGES: LanguageOption[] = [
        { code: 'en', name: 'English', flag: '🇬🇧' },
    ];

    private translate = inject(TranslateService);

    constructor() {
        const codes = LanguageService.LANGUAGES.map((x) => x.code);
        this.translate.addLangs(codes);
        this.translate.setFallbackLang('en');
        this.translate.use('en'); // sử dụng ngôn ngữ khi mở ứng dụng
    }

    // chuyển đổi giữa các ngôn ngữ
    use(lang: string) {
        this.translate.use(lang).subscribe({
            next: (_) => {},
            error: (err) => {
                console.log(err);
            },
        });
    }
}
