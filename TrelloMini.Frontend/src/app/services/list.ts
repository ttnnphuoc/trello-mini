import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { List } from '../models/list';

@Injectable({
  providedIn: 'root'
})
export class ListService {
  private apiUrl = 'http://localhost:5056/api/lists';

  constructor(private http: HttpClient) { }

  getLists(): Observable<List[]> {
    return this.http.get<List[]>(this.apiUrl);
  }

  getList(id: number): Observable<List> {
    return this.http.get<List>(`${this.apiUrl}/${id}`);
  }

  getListsByBoard(boardId: number): Observable<List[]> {
    return this.http.get<List[]>(`${this.apiUrl}/board/${boardId}`);
  }

  createList(list: Partial<List>): Observable<List> {
    return this.http.post<List>(this.apiUrl, list);
  }

  updateList(id: number, list: List): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, list);
  }

  deleteList(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
