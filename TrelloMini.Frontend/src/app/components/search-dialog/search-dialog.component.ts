import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { SearchService, GlobalSearchResponse, CardSearchFilters } from '../../services/search';
import { Board } from '../../models/board';
import { List } from '../../models/list';
import { Card, CardPriority } from '../../models/card';
import { Router } from '@angular/router';

@Component({
  selector: 'app-search-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatTabsModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatCardModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatCheckboxModule
  ],
  templateUrl: './search-dialog.component.html',
  styleUrls: ['./search-dialog.component.scss']
})
export class SearchDialogComponent implements OnInit, OnDestroy {
  searchQuery: string = '';
  searchType: 'all' | 'boards' | 'lists' | 'cards' = 'all';
  loading: boolean = false;
  error: string | null = null;

  // Search results
  boards: Board[] = [];
  lists: List[] = [];
  cards: Card[] = [];
  totalResults: number = 0;

  // Advanced filters for cards
  showAdvancedFilters: boolean = false;
  cardFilters: CardSearchFilters = {
    page: 1,
    pageSize: 20
  };

  priorities = [
    { value: CardPriority.None, label: 'None' },
    { value: CardPriority.Low, label: 'Low' },
    { value: CardPriority.Medium, label: 'Medium' },
    { value: CardPriority.High, label: 'High' },
    { value: CardPriority.Critical, label: 'Critical' }
  ];

  private searchSubject = new Subject<string>();
  private destroy$ = new Subject<void>();

  constructor(
    private searchService: SearchService,
    private router: Router,
    private dialogRef: MatDialogRef<SearchDialogComponent>
  ) { }

  ngOnInit(): void {
    // Debounce search input
    this.searchSubject
      .pipe(
        debounceTime(300),
        distinctUntilChanged()
      )
      .subscribe(query => {
        if (query && query.length >= 2) {
          this.performSearch();
        } else {
          this.clearResults();
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onSearchInput(): void {
    this.searchSubject.next(this.searchQuery);
  }

  performSearch(): void {
    if (!this.searchQuery || this.searchQuery.length < 2) {
      return;
    }

    this.loading = true;
    this.error = null;

    if (this.searchType === 'cards' && this.showAdvancedFilters) {
      // Use advanced card search
      this.cardFilters.query = this.searchQuery;
      this.searchService.searchCards(this.cardFilters).subscribe({
        next: (response) => {
          this.cards = response.cards;
          this.totalResults = response.totalCount;
          this.loading = false;
        },
        error: (error) => {
          console.error('Search error:', error);
          this.error = 'Failed to perform search';
          this.loading = false;
        }
      });
    } else {
      // Use global search
      this.searchService.globalSearch(this.searchQuery, this.searchType).subscribe({
        next: (response: GlobalSearchResponse) => {
          this.boards = response.results.boards;
          this.lists = response.results.lists;
          this.cards = response.results.cards;
          this.totalResults = response.results.totalResults;
          this.loading = false;
        },
        error: (error) => {
          console.error('Search error:', error);
          this.error = 'Failed to perform search';
          this.loading = false;
        }
      });
    }
  }

  clearResults(): void {
    this.boards = [];
    this.lists = [];
    this.cards = [];
    this.totalResults = 0;
  }

  clearSearch(): void {
    this.searchQuery = '';
    this.clearResults();
    this.error = null;
  }

  toggleAdvancedFilters(): void {
    this.showAdvancedFilters = !this.showAdvancedFilters;
  }

  applyFilters(): void {
    this.performSearch();
  }

  clearFilters(): void {
    this.cardFilters = {
      page: 1,
      pageSize: 20
    };
    this.performSearch();
  }

  navigateToBoard(boardId: number): void {
    this.router.navigate(['/boards', boardId]);
    this.dialogRef.close();
  }

  navigateToCard(card: Card): void {
    if (card.list && card.list.board) {
      this.router.navigate(['/boards', card.list.board.id]);
      this.dialogRef.close();
    }
  }

  getPriorityClass(priority: CardPriority): string {
    switch (priority) {
      case CardPriority.Low:
        return 'priority-low';
      case CardPriority.Medium:
        return 'priority-medium';
      case CardPriority.High:
        return 'priority-high';
      case CardPriority.Critical:
        return 'priority-critical';
      default:
        return '';
    }
  }

  getPriorityLabel(priority: CardPriority): string {
    return CardPriority[priority];
  }

  formatDate(date: Date | string | null | undefined): string {
    if (!date) return '';
    const d = new Date(date);
    return d.toLocaleDateString();
  }
}
