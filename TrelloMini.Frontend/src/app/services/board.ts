import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Board } from '../models/board';

@Injectable({
  providedIn: 'root'
})
export class BoardService {
  private apiUrl = 'http://localhost:5063/api/boards';

  constructor(private http: HttpClient) { }

  getBoards(): Observable<Board[]> {
    return this.http.get<Board[]>(this.apiUrl);
  }

  getBoard(id: number): Observable<Board> {
    return this.http.get<Board>(`${this.apiUrl}/${id}`);
  }

  createBoard(board: Partial<Board>): Observable<Board> {
    return this.http.post<Board>(this.apiUrl, board);
  }

  updateBoard(id: number, board: Board): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, board);
  }

  deleteBoard(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
