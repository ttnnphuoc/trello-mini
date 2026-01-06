import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatMenuModule } from '@angular/material/menu';
import { BoardService } from '../../services/board';
import { AuthService } from '../../services/auth';
import { Board } from '../../models/board';
import { User } from '../../models/auth';

@Component({
  selector: 'app-board-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatTooltipModule,
    MatMenuModule
  ],
  templateUrl: './board-list.html',
  styleUrl: './board-list.scss',
})
export class BoardListComponent implements OnInit {
  boards: Board[] = [];
  loading = false;
  currentUser: User | null = null;

  constructor(
    private boardService: BoardService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });
    this.loadBoards();
  }

  loadBoards(): void {
    this.loading = true;
    this.boardService.getBoards().subscribe({
      next: (boards) => {
        this.boards = boards;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading boards:', error);
        this.loading = false;
      }
    });
  }

  createBoard(): void {
    const newBoard = {
      title: 'New Board',
      description: 'A new board for organizing tasks'
    };

    this.boardService.createBoard(newBoard).subscribe({
      next: (board) => {
        this.boards.push(board);
      },
      error: (error) => {
        console.error('Error creating board:', error);
      }
    });
  }

  deleteBoard(id: number): void {
    if (confirm('Are you sure you want to delete this board?')) {
      this.boardService.deleteBoard(id).subscribe({
        next: () => {
          this.boards = this.boards.filter(board => board.id !== id);
        },
        error: (error) => {
          console.error('Error deleting board:', error);
        }
      });
    }
  }

  getTotalCards(board: Board): number {
    return board.lists?.reduce((total, list) => total + (list.cards?.length || 0), 0) || 0;
  }

  logout(): void {
    this.authService.logout();
  }
}
