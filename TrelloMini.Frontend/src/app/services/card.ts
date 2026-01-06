import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Card, MoveCardRequest } from '../models/card';

@Injectable({
  providedIn: 'root'
})
export class CardService {
  private apiUrl = 'http://localhost:5056/api/cards';

  constructor(private http: HttpClient) { }

  getCards(): Observable<Card[]> {
    return this.http.get<Card[]>(this.apiUrl);
  }

  getCard(id: number): Observable<Card> {
    return this.http.get<Card>(`${this.apiUrl}/${id}`);
  }

  getCardsByList(listId: number): Observable<Card[]> {
    return this.http.get<Card[]>(`${this.apiUrl}/list/${listId}`);
  }

  createCard(card: Partial<Card>): Observable<Card> {
    return this.http.post<Card>(this.apiUrl, card);
  }

  updateCard(id: number, card: Partial<Card>): Observable<Card> {
    return this.http.put<Card>(`${this.apiUrl}/${id}`, card);
  }

  moveCard(id: number, moveRequest: MoveCardRequest): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}/move`, moveRequest);
  }

  deleteCard(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
