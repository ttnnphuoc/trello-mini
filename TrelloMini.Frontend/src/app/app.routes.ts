import { Routes } from '@angular/router';
import { BoardListComponent } from './components/board-list/board-list';
import { BoardComponent } from './components/board/board';
import { LoginComponent } from './components/auth/login';
import { RegisterComponent } from './components/auth/register';
import { authGuard, guestGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', component: BoardListComponent },
  { path: 'board/:id', component: BoardComponent, canActivate: [authGuard] },
  { path: 'login', component: LoginComponent, canActivate: [guestGuard] },
  { path: 'register', component: RegisterComponent, canActivate: [guestGuard] },
  { path: '**', redirectTo: '' }
];
