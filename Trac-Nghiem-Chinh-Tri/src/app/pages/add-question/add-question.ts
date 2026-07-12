import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TauriCommandService } from '../../core/services/tauri-command-service';
import { AddNewQuestion } from './models/add-new-question.model';
import { AddNewAnswer } from './models/add-new-answer';
import { ToastStore } from '../../shared/stores/toast.store';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
    selector: 'app-add-question',
    imports: [CommonModule, FormsModule, TranslatePipe],
    templateUrl: './add-question.html',
    styleUrl: './add-question.css',
})
export class AddQuestion {
    questionText = '';
    answers: AddNewAnswer[] = [];
    toastStore = ToastStore;

    constructor(private tauriCommandService: TauriCommandService) {}

    addAnswer() {
        this.answers.push({
            id: Date.now() + Math.random(),
            content: '',
            is_correct: false,
            question_id: -1,
        });
    }

    // chọn đáp án đúng (đảm bảo chỉ một đáp án)
    markCorrect(answerId: number) {
        this.answers.forEach((a) => (a.is_correct = false));
        const found = this.answers.find((a) => a.id === answerId);
        if (found) found.is_correct = true;
    }

    // xóa đáp án
    removeAnswer(index: number) {
        this.answers.splice(index, 1);
    }

    // lưu câu hỏi
    async save() {
        if (!this.questionText.trim()) {
            alert('Vui lòng nhập nội dung câu hỏi.');
            this.toastStore.showToastMessage(true, false, 'Lỗi', 'Vui lòng nhập nội dung câu hỏi');
            return;
        }
        if (this.answers.length < 2) {
            this.toastStore.showToastMessage(true, false, 'Lỗi', 'Cần ít nhất 2 đáp án');
            return;
        }
        if (!this.answers.some((a) => a.is_correct)) {
            this.toastStore.showToastMessage(true, false, 'Lỗi', 'Bạn phải chọn 1 đáp án đúng');
            return;
        }

        const newQuestion: AddNewQuestion = { content: this.questionText };
        const newQuestionId = await this.tauriCommandService.invokeCommand<number>(
            TauriCommandService.ADD_QUESTION_COMMAND,
            { newQuestion: newQuestion }
        );

        if (!newQuestionId) {
            return;
        }

        //add_answers
        for (let a of this.answers) {
            a.question_id = newQuestionId;
        }

        const isSuccess = await this.tauriCommandService.invokeCommand<boolean>(
            TauriCommandService.ADD_ANSWERS_COMMAND,
            { newAnswers: this.answers }
        );

        if (isSuccess) {
            this.toastStore.showToastMessage(true, true, 'Success', '');
        }

        // Reset form
        this.questionText = '';
        this.answers = [];
    }
}
