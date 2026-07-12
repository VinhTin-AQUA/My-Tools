import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LayoutRoutes, MainRoutes } from '../../core/constants/routes-consts';
import { Router, RouterLink } from '@angular/router';
import { TauriCommandService } from '../../core/services/tauri-command-service';
import { TranslatePipe } from '@ngx-translate/core';
import { Question } from '../../core/models/question';
import { Answer } from '../../core/models/answer';
import { QuestionCancelDialog } from '../../shared/components/question-cancel-dialog/question-cancel-dialog';

@Component({
    selector: 'app-question-bank',
    imports: [CommonModule, FormsModule, RouterLink, TranslatePipe, QuestionCancelDialog],
    templateUrl: './question-bank.html',
    styleUrl: './question-bank.css',
})
export class QuestionBank {
    mainRoutes = MainRoutes;
    questions = signal<Question[]>([]);
    searchTerm = '';
    showDetail = false;
    selectedQuestion = signal<Question | null>(null);
    isShowQuestionDialog = signal<boolean>(false);
    questionIdToDelete: number | null = null;

    constructor(private tauriCommandService: TauriCommandService, private router: Router) {}

    async ngOnInit() {
        let questions = await this.tauriCommandService.invokeCommand<Question[]>(
            TauriCommandService.GET_ALL_QUESTIONS_COMMAND,
            {}
        );

        if (!questions) {
            return;
        }
        this.questions.set(questions);

        for (let q of this.questions()) {
            const answers = await this.tauriCommandService.invokeCommand<Answer[]>(
                TauriCommandService.GET_ANSWERS_BY_QUESTION_ID_COMMAND,
                { questionId: q.id }
            );

            if (!answers) {
                continue;
            }
            q.answers = answers;
        }
    }

    openDetail(q: Question) {
        this.selectedQuestion.set(q);
        this.showDetail = true;
    }

    onShowDelete(q: Question) {
        this.isShowQuestionDialog.set(true);
        this.questionIdToDelete = q.id;
    }

    async onShowQuestionDialogClose(result: boolean) {
        this.isShowQuestionDialog.set(false);

        if (!result) {
            this.questionIdToDelete = null;
            return;
        }

        if (!this.questionIdToDelete) {
            return;
        }

        const r1 = await this.tauriCommandService.invokeCommand<boolean>(
            TauriCommandService.DELETE_QUESTION_BY_ID_COMMAND,
            {
                questionId: this.questionIdToDelete,
            }
        );

        const r2 = await this.tauriCommandService.invokeCommand<boolean>(
            TauriCommandService.DELETE_ANSWERS_BY_QUESTION_ID_COMMAND,
            {
                questionId: this.questionIdToDelete,
            }
        );

        if (r1 && r2) {
            this.questions.update((questions) =>
                questions.filter((q) => q.id !== this.questionIdToDelete)
            );

            this.questionIdToDelete = null;
        }
    }

    onEditQuetion(q: Question) {
        this.router.navigateByUrl(`/${MainRoutes.EDIT_QUESTION}/${q.id}`);
    }
}
