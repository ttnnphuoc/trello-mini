import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Board } from '../models/board';
import { List } from '../models/list';
import { Card, CardPriority } from '../models/card';

export interface GlobalSearchResponse {
  query: string;
  results: {
    boards: Board[];
    lists: List[];
    cards: Card[];
    totalResults: number;
  };
  page: number;
  pageSize: number;
}

export interface SearchCardsResponse {
  cards: Card[];
  totalCount: number;
  page: number;
  pageSize: number;
  hasMore: boolean;
}

export interface SearchBoardsResponse {
  boards: Board[];
  totalCount: number;
  page: number;
  pageSize: number;
  hasMore: boolean;
}

export interface CardSearchFilters {
  query?: string;
  boardId?: number;
  listId?: number;
  priority?: CardPriority;
  dueDateStart?: Date;
  dueDateEnd?: Date;
  overdue?: boolean;
  page?: number;
  pageSize?: number;
}

@Injectable({
  providedIn: 'root'
})
export class SearchService {
  private apiUrl = 'http://localhost:5056/api/Search';

  constructor(private http: HttpClient) { }

  globalSearch(
    query: string,
    searchType?: 'all' | 'boards' | 'lists' | 'cards',
    page: number = 1,
    pageSize: number = 20
  ): Observable<GlobalSearchResponse> {
    let params = new HttpParams()
      .set('query', query)
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (searchType) {
      params = params.set('searchType', searchType);
    }

    return this.http.get<GlobalSearchResponse>(`${this.apiUrl}/global`, { params });
  }

  searchCards(filters: CardSearchFilters): Observable<SearchCardsResponse> {
    let params = new HttpParams();

    if (filters.query) {
      params = params.set('query', filters.query);
    }

    if (filters.boardId !== undefined) {
      params = params.set('boardId', filters.boardId.toString());
    }

    if (filters.listId !== undefined) {
      params = params.set('listId', filters.listId.toString());
    }

    if (filters.priority !== undefined) {
      params = params.set('priority', filters.priority.toString());
    }

    if (filters.dueDateStart) {
      params = params.set('dueDateStart', filters.dueDateStart.toISOString());
    }

    if (filters.dueDateEnd) {
      params = params.set('dueDateEnd', filters.dueDateEnd.toISOString());
    }

    if (filters.overdue !== undefined) {
      params = params.set('overdue', filters.overdue.toString());
    }

    if (filters.page !== undefined) {
      params = params.set('page', filters.page.toString());
    }

    if (filters.pageSize !== undefined) {
      params = params.set('pageSize', filters.pageSize.toString());
    }

    return this.http.get<SearchCardsResponse>(`${this.apiUrl}/cards`, { params });
  }

  searchBoards(query: string, page: number = 1, pageSize: number = 20): Observable<SearchBoardsResponse> {
    const params = new HttpParams()
      .set('query', query)
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<SearchBoardsResponse>(`${this.apiUrl}/boards`, { params });
  }
}
