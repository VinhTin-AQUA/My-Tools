import { Routes } from '@angular/router';
import { MainLayout } from './shared/layouts/main-layout/main-layout';
import { MainRoutes } from './core/constants/routes-consts';
import { Home } from './pages/home/home';
import { StartExam } from './pages/start-exam/start-exam';
import { QuestionBank } from './pages/question-bank/question-bank';
import { AddQuestion } from './pages/add-question/add-question';
import { Settings } from './pages/settings/settings';
import { EditQuestion } from './pages/edit-question/edit-question';

export const routes: Routes = [
    {
        path: '',
        component: MainLayout,
        children: [
            {
                path: MainRoutes.HOME,
                component: Home,
            },
            {
                path: MainRoutes.START_EXAM,
                component: StartExam,
            },
            {
                path: MainRoutes.QUESTION_BANK,
                component: QuestionBank,
            },
            {
                path: MainRoutes.ADD_QUESTION,
                component: AddQuestion,
            },
            {
                path: MainRoutes.EDIT_QUESTION + '/:id',
                component: EditQuestion,
            },
            {
                path: MainRoutes.SETTINGS,
                component: Settings,
            },
            { path: '', redirectTo: MainRoutes.HOME, pathMatch: 'full' },
        ],
    },
    { path: '**', redirectTo: '' },
];
