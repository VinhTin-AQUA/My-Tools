export class SpreadSheetHelper {
    static extractSpreadsheetId(url: string): string | null {
        const match = url.match(/\/spreadsheets\/d\/([a-zA-Z0-9-_]+)/);
        return match ? match[1] : null;
    }
}
