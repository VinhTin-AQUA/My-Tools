import { CommonModule } from '@angular/common';
import { Component, computed, HostListener, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { Question } from '../../core/models/question';
import { TauriCommandService } from '../../core/services/tauri-command-service';
import { ToastStore } from '../../shared/stores/toast.store';
import { Answer } from '../../core/models/answer';

@Component({
    selector: 'app-start-exam',
    imports: [CommonModule, FormsModule, TranslatePipe],
    templateUrl: './start-exam.html',
    styleUrl: './start-exam.css',
})
export class StartExam {
    totalQuestionPerExam = signal<number>(20);
    toastStore = ToastStore;
    questions = signal<Question[]>([]);

    selectedAnswers: { [questionId: number]: number } = {};
    submitted = false;
    score = 0;
    correctCount = 0;
    examDuration = signal<number>(10); // phút
    remainingSeconds = signal<number>(10 * 60);
    timerInterval: any = null;
    formattedTime = computed(() => {
        const sec = this.remainingSeconds();
        const m = Math.floor(sec / 60);
        const s = sec % 60;
        return `${m}:${s.toString().padStart(2, '0')}`;
    });
    isStarted = signal<boolean>(false);

    constructor(private tauriCommandService: TauriCommandService) {}

    async ngOnInit() {
        await this.onRandomQuestions();
        // window.addEventListener('beforeunload', this.beforeUnloadHandler);
    }

    ngOnDestroy() {
        // window.removeEventListener('beforeunload', this.beforeUnloadHandler);
        if (this.timerInterval) {
            clearInterval(this.timerInterval);
        }
    }

    //=========================================

    async onRandomQuestions() {
        const questions = await this.tauriCommandService.invokeCommand<Question[]>(
            TauriCommandService.GET_RANDOM_QUESTIONS_COMMAND,
            { rand: this.totalQuestionPerExam() }
        );

        if (!questions) {
            this.toastStore.showToastMessage(true, false, 'Error', 'Cannot get quesitons');
            return;
        }
        this.questions.set(questions);

        for (let q of this.questions()) {
            const answers = await this.tauriCommandService.invokeCommand<Answer[]>(
                TauriCommandService.GET_ANSWERS_BY_QUESTION_ID_COMMAND,
                { questionId: q.id }
            );

            if (!answers) {
                q.answers = [];
            } else {
                q.answers = answers;
            }
        }

        this.selectedAnswers = {};
        this.submitted = false;
        this.score = 0;
        this.correctCount = 0;
        this.isStarted.set(false);
        if (this.timerInterval) {
            clearInterval(this.timerInterval);
        }
        this.remainingSeconds.set(this.examDuration() * 60);
    }

    onInputTotalQuestionPerExam(num: number) {
        this.totalQuestionPerExam.set(num);
    }

    onChangeExamDuration(value: number) {
        this.examDuration.set(value);
        this.remainingSeconds.set(this.examDuration() * 60);
    }

    checkStartExam(event: Event) {
        if (this.isStarted() == false) {
            event.preventDefault();
            event.stopPropagation();

            this.toastStore.showToastMessage(
                true,
                false,
                'Chưa bắt đầu thi',
                'Vui lòng bắt đầu bài thi'
            );
        }
    }

    private startTimer() {
        if (this.timerInterval) {
            clearInterval(this.timerInterval);
        }

        this.remainingSeconds.set(this.examDuration() * 60);

        this.timerInterval = setInterval(() => {
            const current = this.remainingSeconds();

            if (current <= 1) {
                clearInterval(this.timerInterval);
                this.remainingSeconds.set(0);
                this.autoSubmit();
            } else {
                this.remainingSeconds.set(current - 1);
            }
        }, 1000);
    }

    startExam() {
        this.submitted = false;
        this.selectedAnswers = {};
        this.isStarted.set(true);
        this.startTimer();
        this.toastStore.showToastMessage(true, true, 'Bắt đầu', 'Bắt đầu bài thi');
    }

    submit() {
        this.toastStore.showToastMessage(true, true, 'Kết thúc', 'Kết thúc bài thi');
        this.isStarted.set(false);
        this.submitted = true;

        if (this.timerInterval) {
            clearInterval(this.timerInterval);
        }

        this.correctCount = 0;

        for (const q of this.questions()) {
            const selectedId = this.selectedAnswers[q.id];
            const selectedAnswer = q.answers.find((a) => a.id === selectedId);
            if (selectedAnswer?.is_correct) {
                this.correctCount++;
            }
        }

        const total = this.questions().length;
        this.score = total > 0 ? Math.round((this.correctCount / total) * 10 * 100) / 100 : 0;
    }

    autoSubmit() {
        if (!this.submitted) {
            this.submit();
        }
    }
}
