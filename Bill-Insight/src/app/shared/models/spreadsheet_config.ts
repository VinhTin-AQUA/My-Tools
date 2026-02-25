export interface SpreadsheetConfigModel {
    spreadSheetUrl: string;
    spreadSheetId: string;
    workingSheet: WorkingSheetModel;
}

export interface WorkingSheetModel {
    id: number;
    title: string;
    isActive: boolean;
}
