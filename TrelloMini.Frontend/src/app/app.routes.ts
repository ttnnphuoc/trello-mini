import { Routes } from '@angular/router';
import { BoardListComponent } from './components/board-list/board-list';
import { BoardComponent } from './components/board/board';

export const routes: Routes = [
  { path: '', component: BoardListComponent },
  { path: 'board/:id', component: BoardComponent },
  { path: '**', redirectTo: '' }
];
