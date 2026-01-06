import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { BoardService } from '../../services/board';
import { ListService } from '../../services/list';
import { CardService } from '../../services/card';
import { Board } from '../../models/board';
import { List } from '../../models/list';
import { Card } from '../../models/card';

@Component({
  selector: 'app-board',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule
  ],
  templateUrl: './board.html',
  styleUrl: './board.scss',
})
export class BoardComponent implements OnInit {
  board: Board | null = null;
  loading = false;
  newListTitle = '';
  newCardTitles: { [listId: number]: string } = {};

  constructor(
    private route: ActivatedRoute,
    private boardService: BoardService,
    private listService: ListService,
    private cardService: CardService
  ) {}

  ngOnInit(): void {
    const boardId = Number(this.route.snapshot.paramMap.get('id'));
    if (boardId) {
      this.loadBoard(boardId);
    }
  }

  loadBoard(id: number): void {
    this.loading = true;
    this.boardService.getBoard(id).subscribe({
      next: (board) => {
        this.board = board;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading board:', error);
        this.loading = false;
      }
    });
  }

  createList(): void {
    if (!this.board || !this.newListTitle.trim()) return;

    const newList = {
      title: this.newListTitle.trim(),
      position: this.board.lists.length,
      boardId: this.board.id
    };

    this.listService.createList(newList).subscribe({
      next: (list) => {
        this.board!.lists.push(list);
        this.newListTitle = '';
      },
      error: (error) => {
        console.error('Error creating list:', error);
      }
    });
  }

  createCard(listId: number): void {
    const cardTitle = this.newCardTitles[listId];
    if (!cardTitle?.trim()) return;

    const list = this.board?.lists.find(l => l.id === listId);
    if (!list) return;

    const newCard = {
      title: cardTitle.trim(),
      position: list.cards.length,
      listId: listId
    };

    this.cardService.createCard(newCard).subscribe({
      next: (card) => {
        list.cards.push(card);
        this.newCardTitles[listId] = '';
      },
      error: (error) => {
        console.error('Error creating card:', error);
      }
    });
  }

  deleteList(listId: number): void {
    if (!confirm('Are you sure you want to delete this list?')) return;

    this.listService.deleteList(listId).subscribe({
      next: () => {
        this.board!.lists = this.board!.lists.filter(list => list.id !== listId);
      },
      error: (error) => {
        console.error('Error deleting list:', error);
      }
    });
  }

  deleteCard(listId: number, cardId: number): void {
    if (!confirm('Are you sure you want to delete this card?')) return;

    this.cardService.deleteCard(cardId).subscribe({
      next: () => {
        const list = this.board?.lists.find(l => l.id === listId);
        if (list) {
          list.cards = list.cards.filter(card => card.id !== cardId);
        }
      },
      error: (error) => {
        console.error('Error deleting card:', error);
      }
    });
  }
}
