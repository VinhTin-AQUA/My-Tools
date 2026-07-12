import { Injectable } from '@angular/core';
import { invoke } from '@tauri-apps/api/core';
import { DialogService } from './dialog-service';

@Injectable({
    providedIn: 'root',
})
export class TauriCommandService {
    public static readonly ADD_QUESTION_COMMAND = 'add_question';
    public static readonly ADD_ANSWERS_COMMAND = 'add_answers';
    public static readonly GET_ALL_QUESTIONS_COMMAND = 'get_all_questions';
    public static readonly GET_ANSWERS_BY_QUESTION_ID_COMMAND = 'get_answers_by_question_id';
    public static readonly DELETE_QUESTION_BY_ID_COMMAND = 'delete_question_by_id';
    public static readonly DELETE_ANSWERS_BY_QUESTION_ID_COMMAND = 'delete_answers_by_question_id';
    public static readonly DELETE_ANSWER_BY_ID_COMMAND = 'delete_answer_by_id';
    public static readonly UPDATE_QUESTION_BY_ID_COMMAND = 'update_question_by_id';
    public static readonly UPDATE_ANSWERS_COMMAND = 'update_answers';
    public static readonly GET_QUESTION_BY_ID_COMMAND = 'get_question_by_id';
    public static readonly GET_TOTAL_QUESTIONS_COMMAND = 'get_total_questions';
    public static readonly GET_RANDOM_QUESTIONS_COMMAND = 'get_random_questions';
    public static readonly EXPOT_JSON_DATA_COMMAND = 'expot_json_data';
    public static readonly IMPORT_JSON_TO_DB_COMMAND = 'import_json_to_db';
    public static readonly DELETE_ALL_DATA_COMMAND = 'delete_all_data';
    
    constructor(private dialogService: DialogService) {}

    async invokeCommand<T>(cmd: string, params: any): Promise<T | null> {
        try {
            this.dialogService.showLoadingDialog(true);
            const response = await invoke<T>(cmd, params);
            this.dialogService.showLoadingDialog(false);

            return response;
        } catch (e) {
            alert(e);
            console.log(e);
            this.dialogService.showLoadingDialog(false);

            return null;
        }
    }
}
