import { signalStore, withState, withMethods, patchState } from '@ngrx/signals';
import { SpreadsheetConfigModel } from '../../core/models/spreadsheet-config';

const initialState: SpreadsheetConfigModel = {
    spreadSheetId: '',
    spreadSheetUrl: '',
    workingSheet: {
        id: -1,
        isActive: false,
        title: '',
    },
};

export const SpreadsheetConfigStore = signalStore(
    {
        providedIn: 'root',
    },
    withState(initialState),
    withMethods((store) => {
        function update(model: SpreadsheetConfigModel) {
            patchState(store, (currentState) => ({
                spreadSheetId: model.spreadSheetId,
                spreadSheetUrl: model.spreadSheetUrl,
                workingSheet: {
                    id: model.workingSheet.id,
                    isActive: model.workingSheet.isActive,
                    title: model.workingSheet.title,
                },
            }));
        }

        function updateWorkingSheet(id: number, title: string) {
            patchState(store, (currentState) => ({
                workingSheet: {
                    id: id,
                    isActive: true,
                    title: title,
                },
            }));
        }

        return {
            update,
            updateWorkingSheet,
        };
    })
);
