import { Component, signal } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { TauriCommandService } from '../../core/services/tauri-command-service';
import { MainRoutes } from '../../core/constants/routes-consts';
import { RouterLink } from '@angular/router';

@Component({
    selector: 'app-home',
    imports: [TranslatePipe, RouterLink],
    templateUrl: './home.html',
    styleUrl: './home.css',
})
export class Home {
    totalQuestions = signal<number>(0);
    totalQuestionsPerExam = signal<number>(20);
    mainbRoutes = MainRoutes;

    constructor(private tauriCommandService: TauriCommandService) {}

    async ngOnInit() {
        let totalQuestions = await this.tauriCommandService.invokeCommand<number>(
            TauriCommandService.GET_TOTAL_QUESTIONS_COMMAND,
            {}
        );

        if (totalQuestions) {
            this.totalQuestions.set(totalQuestions);
        }
    }
}

//
