import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { BoardService } from '../../services/board';
import { ListService } from '../../services/list';
import { CardService } from '../../services/card';
import { Board } from '../../models/board';
import { List } from '../../models/list';
import { Card, CardPriority } from '../../models/card';
import { CardDetailsComponent } from '../card-details/card-details';

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
    MatFormFieldModule,
    MatTooltipModule,
    MatDialogModule,
    DragDropModule
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
    private cardService: CardService,
    private dialog: MatDialog
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

  onCardDropped(event: CdkDragDrop<Card[]>): void {
    if (!this.board) return;

    if (event.previousContainer === event.container) {
      // Reordering within same list
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
      this.updateCardPositions(event.container.data);
    } else {
      // Moving between different lists
      const card = event.previousContainer.data[event.previousIndex];
      const targetListId = this.getListIdFromContainer(event.container);
      
      if (targetListId) {
        transferArrayItem(
          event.previousContainer.data,
          event.container.data,
          event.previousIndex,
          event.currentIndex
        );
        
        this.moveCard(card.id, targetListId, event.currentIndex);
      }
    }
  }

  private getListIdFromContainer(container: any): number | null {
    // Extract list ID from container ID (format: 'list-{listId}')
    const containerId = container.id;
    const match = containerId.match(/list-(\d+)/);
    return match ? parseInt(match[1]) : null;
  }

  private updateCardPositions(cards: Card[]): void {
    cards.forEach((card, index) => {
      if (card.position !== index) {
        card.position = index;
        this.cardService.updateCard(card.id, card).subscribe({
          error: (error) => console.error('Error updating card position:', error)
        });
      }
    });
  }

  private moveCard(cardId: number, targetListId: number, newPosition: number): void {
    const moveRequest = {
      listId: targetListId,
      position: newPosition
    };

    this.cardService.moveCard(cardId, moveRequest).subscribe({
      next: () => {
        // Update the card's listId in the local data
        this.board?.lists.forEach(list => {
          const cardIndex = list.cards.findIndex(c => c.id === cardId);
          if (cardIndex >= 0) {
            list.cards[cardIndex].listId = targetListId;
            list.cards[cardIndex].position = newPosition;
          }
        });
      },
      error: (error) => {
        console.error('Error moving card:', error);
        // Revert the UI change on error
        this.loadBoard(this.board!.id);
      }
    });
  }

  openCardDetails(card: Card, event: Event): void {
    // Prevent opening dialog when dragging
    if ((event.target as HTMLElement).closest('.drag-handle')) {
      return;
    }

    const dialogRef = this.dialog.open(CardDetailsComponent, {
      width: '600px',
      maxWidth: '90vw',
      data: { card }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && result.deleted) {
        // Card was deleted, remove from UI
        this.board?.lists.forEach(list => {
          list.cards = list.cards.filter(c => c.id !== card.id);
        });
      } else if (result) {
        // Card was updated, update in UI
        this.board?.lists.forEach(list => {
          const cardIndex = list.cards.findIndex(c => c.id === card.id);
          if (cardIndex >= 0) {
            list.cards[cardIndex] = { ...list.cards[cardIndex], ...result };
          }
        });
      }
    });
  }

  getPriorityColor(priority: CardPriority): string {
    switch (priority) {
      case CardPriority.Low: return 'green';
      case CardPriority.Medium: return 'yellow';
      case CardPriority.High: return 'orange';
      case CardPriority.Critical: return 'red';
      default: return 'gray';
    }
  }

  getPriorityLabel(priority: CardPriority): string {
    switch (priority) {
      case CardPriority.Low: return 'Low';
      case CardPriority.Medium: return 'Medium';
      case CardPriority.High: return 'High';
      case CardPriority.Critical: return 'Critical';
      default: return 'None';
    }
  }

  getDueDateClass(dueDate: Date | string): string {
    const due = new Date(dueDate);
    const now = new Date();
    const diffDays = Math.ceil((due.getTime() - now.getTime()) / (1000 * 60 * 60 * 24));

    if (diffDays < 0) {
      return 'text-red-600'; // Overdue
    } else if (diffDays <= 1) {
      return 'text-orange-600'; // Due today or tomorrow
    } else {
      return 'text-gray-600'; // Future due date
    }
  }
}
