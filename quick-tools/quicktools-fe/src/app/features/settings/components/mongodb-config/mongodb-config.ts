import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { WebuiService } from '../../../../services/webui-service';
import { WebUIResponse } from '../../../../models/webui.response';
import { MongoDBSetting } from '../../../../models/mongo-DBSetting.model';
import { MessageService } from '@openng/optimus-ui/api';
import { ToastModule } from '@openng/optimus-ui/toast';

@Component({
    selector: 'app-mongodb-config',
    imports: [FormsModule, ToastModule],
    templateUrl: './mongodb-config.html',
    styleUrl: './mongodb-config.css',
    providers: [MessageService],
})
export class MongodbConfig {
    connectionString: string = '';
    databaseName: string = '';

    private messageService = inject(MessageService);

    constructor(private webUIService: WebuiService) {}

    async onSubmit() {
        console.log('Connection String:', this.connectionString);
        console.log('Database Name:', this.databaseName);

        const r = await this.webUIService.callJson<WebUIResponse<MongoDBSetting>>(
            'setMongoDBSetting',
            {
                connectionString: this.connectionString,
                databaseName: this.databaseName,
            },
        );

        console.log(r);
        

        this.messageService.add({
            severity: 'success',
            summary: r.title,
            detail: r.description,
        });
    }
}
