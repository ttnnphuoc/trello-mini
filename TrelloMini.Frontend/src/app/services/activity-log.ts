import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ActivityLog, ActivityLogResponse, ActivityType } from '../models/activity-log';

@Injectable({
  providedIn: 'root'
})
export class ActivityLogService {
  private apiUrl = 'http://localhost:5056/api/ActivityLogs';

  constructor(private http: HttpClient) { }

  getBoardActivityLogs(
    boardId: number,
    page: number = 1,
    pageSize: number = 50,
    activityType?: ActivityType,
    startDate?: Date,
    endDate?: Date
  ): Observable<ActivityLogResponse> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (activityType !== undefined) {
      params = params.set('activityType', activityType.toString());
    }

    if (startDate) {
      params = params.set('startDate', startDate.toISOString());
    }

    if (endDate) {
      params = params.set('endDate', endDate.toISOString());
    }

    return this.http.get<ActivityLogResponse>(`${this.apiUrl}/board/${boardId}`, { params });
  }

  getCardActivityLogs(cardId: number): Observable<ActivityLog[]> {
    return this.http.get<ActivityLog[]>(`${this.apiUrl}/card/${cardId}`);
  }

  getMyActivityLogs(page: number = 1, pageSize: number = 50): Observable<ActivityLogResponse> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<ActivityLogResponse>(`${this.apiUrl}/user/me`, { params });
  }

  getActivityLog(id: number): Observable<ActivityLog> {
    return this.http.get<ActivityLog>(`${this.apiUrl}/${id}`);
  }

  createActivityLog(activityLog: Partial<ActivityLog>): Observable<ActivityLog> {
    return this.http.post<ActivityLog>(this.apiUrl, activityLog);
  }

  deleteActivityLog(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  // Helper method to format activity descriptions
  getActivityIcon(activityType: ActivityType): string {
    switch (activityType) {
      case ActivityType.BoardCreated:
      case ActivityType.BoardUpdated:
        return 'dashboard';
      case ActivityType.BoardDeleted:
        return 'delete';
      case ActivityType.BoardMemberAdded:
      case ActivityType.UserJoined:
        return 'person_add';
      case ActivityType.BoardMemberRemoved:
      case ActivityType.UserLeft:
        return 'person_remove';
      case ActivityType.BoardMemberRoleChanged:
        return 'admin_panel_settings';
      case ActivityType.ListCreated:
        return 'add_box';
      case ActivityType.ListUpdated:
      case ActivityType.ListMoved:
        return 'view_week';
      case ActivityType.ListDeleted:
        return 'delete';
      case ActivityType.CardCreated:
        return 'note_add';
      case ActivityType.CardUpdated:
        return 'edit_note';
      case ActivityType.CardDeleted:
        return 'delete';
      case ActivityType.CardMoved:
        return 'drag_indicator';
      case ActivityType.CardPriorityChanged:
        return 'flag';
      case ActivityType.CardDueDateChanged:
        return 'schedule';
      case ActivityType.CardAssigned:
      case ActivityType.CardUnassigned:
        return 'assignment_ind';
      case ActivityType.CommentAdded:
      case ActivityType.CommentUpdated:
        return 'comment';
      case ActivityType.CommentDeleted:
        return 'delete';
      case ActivityType.AttachmentAdded:
        return 'attach_file';
      case ActivityType.AttachmentDeleted:
        return 'delete';
      default:
        return 'info';
    }
  }

  getActivityColor(activityType: ActivityType): string {
    switch (activityType) {
      case ActivityType.BoardCreated:
      case ActivityType.ListCreated:
      case ActivityType.CardCreated:
      case ActivityType.BoardMemberAdded:
      case ActivityType.UserJoined:
      case ActivityType.CommentAdded:
      case ActivityType.AttachmentAdded:
        return 'primary';
      case ActivityType.BoardUpdated:
      case ActivityType.ListUpdated:
      case ActivityType.CardUpdated:
      case ActivityType.CardMoved:
      case ActivityType.ListMoved:
      case ActivityType.BoardMemberRoleChanged:
        return 'accent';
      case ActivityType.BoardDeleted:
      case ActivityType.ListDeleted:
      case ActivityType.CardDeleted:
      case ActivityType.BoardMemberRemoved:
      case ActivityType.UserLeft:
      case ActivityType.CommentDeleted:
      case ActivityType.AttachmentDeleted:
        return 'warn';
      case ActivityType.CardPriorityChanged:
        return 'warn';
      case ActivityType.CardDueDateChanged:
        return 'accent';
      default:
        return '';
    }
  }
}
