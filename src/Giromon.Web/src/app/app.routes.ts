import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  {
    path: 'entrar',
    canActivate: [guestGuard],
    loadComponent: () => import('./features/authentication/pages/login/login.component').then(c => c.LoginComponent)
  },
  {
    path: 'criar-conta',
    canActivate: [guestGuard],
    loadComponent: () => import('./features/authentication/pages/register/register.component').then(c => c.RegisterComponent)
  },
  {
    path: 'jogar',
    canActivate: [authGuard],
    loadComponent: () => import('./features/game/pages/slot-machine/slot-machine.component').then(c => c.SlotMachineComponent)
  },
  { path: '', pathMatch: 'full', redirectTo: 'jogar' },
  { path: '**', redirectTo: 'jogar' }
];
