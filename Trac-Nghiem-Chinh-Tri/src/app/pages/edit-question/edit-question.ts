import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { AddNewAnswer } from '../add-question/models/add-new-answer';
import { ToastStore } from '../../shared/stores/toast.store';
import { TauriCommandService } from '../../core/services/tauri-command-service';
import { Answer } from '../../core/models/answer';
import { Question } from '../../core/models/question';
import { UpdateQuestion } from './models/update-question';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-edit-question',
    imports: [CommonModule, FormsModule, TranslatePipe],
    templateUrl: './edit-question.html',
    styleUrl: './edit-question.css',
})
export class EditQuestion {
    question = signal<Question | null>(null);
    newAnswers = signal<AddNewAnswer[]>([]);
    oldAnswers = signal<Answer[]>([]);

    toastStore = ToastStore;

    constructor(
        private tauriCommandService: TauriCommandService,
        private activatedRoute: ActivatedRoute
    ) {}

    // them, sua, xoa answer moi
    // them sua xoa answer cu

    ngOnInit() {
        this.activatedRoute.params.subscribe({
            next: async (params: any) => {
                console.log(params.id); // {id: '2', name: 'hoc'}
                const questionId = Number(params.id);

                await this.getQuestionById(questionId);
                await this.getAnswersByQuestionId(questionId);
            },
        });
    }

    private async getQuestionById(questionId: number) {
        const question = await this.tauriCommandService.invokeCommand<Question>(
            TauriCommandService.GET_QUESTION_BY_ID_COMMAND,
            { questionId: questionId }
        );

        if (question) {
            this.question.set(question);
        }
    }

    private async getAnswersByQuestionId(questionId: number) {
        const answers = await this.tauriCommandService.invokeCommand<Answer[]>(
            TauriCommandService.GET_ANSWERS_BY_QUESTION_ID_COMMAND,
            { questionId: questionId }
        );

        if (answers) {
            this.oldAnswers.set(answers);
        }
    }

    //==============================

    updateQuestionContent(content: string) {
        this.question.update((q) => (q ? { ...q, content } : q));
    }

    //===================

    addNewAnswer() {
        this.newAnswers.update((ans) => {
            ans.push({
                id: Date.now() + Math.random(),
                content: '',
                is_correct: false,
                question_id: -1,
            });
            return ans;
        });
    }

    markCorrectNewAnswer(answerId: number) {
        this.newAnswers.update((answers) =>
            answers.map((a) => ({
                ...a,
                is_correct: a.id === answerId,
            }))
        );

        this.oldAnswers.update((answers) =>
            answers.map((a) => ({
                ...a,
                is_correct: false,
            }))
        );
    }

    removeNewAnswer(answerId: number) {
        this.newAnswers.update((answers) => answers.filter((a) => a.id !== answerId));
    }

    //===============\

    markCorrectOldAnswer(answerId: number) {
        this.oldAnswers.update((answers) =>
            answers.map((a) => ({
                ...a,
                is_correct: a.id === answerId,
            }))
        );

        this.newAnswers.update((answers) =>
            answers.map((a) => ({
                ...a,
                is_correct: false,
            }))
        );
    }

    async removeOldAnswer(answerId: number) {
        const r = await this.tauriCommandService.invokeCommand<boolean>(
            TauriCommandService.DELETE_ANSWER_BY_ID_COMMAND,
            { id: answerId }
        );

        if (!r) {
            return;
        }
        this.oldAnswers.update((answers) => answers.filter((a) => a.id !== answerId));
    }

    // lưu câu hỏi
    async save() {
        if (!this.question()) {
            this.toastStore.showToastMessage(true, false, 'Lỗi', 'Somethine error');
            return;
        }

        if (!this.question()?.content.trim()) {
            alert('Vui lòng nhập nội dung câu hỏi.');
            this.toastStore.showToastMessage(true, false, 'Lỗi', 'Vui lòng nhập nội dung câu hỏi');
            return;
        }
        if (this.newAnswers().length + this.oldAnswers().length < 2) {
            this.toastStore.showToastMessage(true, false, 'Lỗi', 'Cần ít nhất 2 đáp án');
            return;
        }

        const hasCorrectNew = this.newAnswers().some((a) => a.is_correct);
        const hasCorrectOld = this.oldAnswers().some((a) => a.is_correct);

        if (!hasCorrectNew && !hasCorrectOld) {
            this.toastStore.showToastMessage(
                true,
                false,
                'Lỗi',
                'Bạn phải chọn ít nhất 1 đáp án đúng'
            );
            return;
        }

        const updateQuestion: UpdateQuestion = {
            id: this.question()!.id,
            content: this.question()!.content,
        };
        await this.tauriCommandService.invokeCommand(
            TauriCommandService.UPDATE_QUESTION_BY_ID_COMMAND,
            { question: updateQuestion }
        );

        // update old answer
        await this.tauriCommandService.invokeCommand(TauriCommandService.UPDATE_ANSWERS_COMMAND, {
            answers: this.oldAnswers(),
        });

        //add new answers
        for (let a of this.newAnswers()) {
            a.question_id = this.question()!.id;
        }

        const isSuccess = await this.tauriCommandService.invokeCommand<boolean>(
            TauriCommandService.ADD_ANSWERS_COMMAND,
            { newAnswers: this.newAnswers() }
        );

        if (isSuccess) {
            this.toastStore.showToastMessage(true, true, 'Success', '');
        }
    }
}
