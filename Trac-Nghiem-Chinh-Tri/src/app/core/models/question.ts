import { Answer } from './answer';

export interface Question {
    id: number;
    content: string;
    answers: Answer[];
}
