import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';

export const routes: Routes = [
    {
        path: 'home',
        loadComponent: ()=> import('./components/home/home.component')
            .then( m => m.HomeComponent),
        data: { preload: false}
    },
    {
        path: 'auth-redirect',
        component: LoginComponent
    },
    {
        path: 'login',
        component: LoginComponent
    },
    {
        path: '',
        component: LoginComponent
    }
];
