import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MongodbConfig } from '../../components/mongodb-config/mongodb-config';

@Component({
    selector: 'app-configs',
    imports: [CommonModule, MongodbConfig],
    templateUrl: './configs.html',
    styleUrl: './configs.css',
})
export class Configs {}
