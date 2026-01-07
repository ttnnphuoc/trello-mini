import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { ActivityLog, ActivityType } from '../../models/activity-log';
import { ActivityLogService } from '../../services/activity-log';

@Component({
  selector: 'app-activity-timeline',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatChipsModule
  ],
  templateUrl: './activity-timeline.component.html',
  styleUrls: ['./activity-timeline.component.scss']
})
export class ActivityTimelineComponent implements OnInit {
  @Input() boardId?: number;
  @Input() cardId?: number;
  @Input() showUserActivities: boolean = false;

  activities: ActivityLog[] = [];
  loading: boolean = false;
  error: string | null = null;
  page: number = 1;
  pageSize: number = 20;
  hasMore: boolean = false;

  constructor(private activityLogService: ActivityLogService) { }

  ngOnInit(): void {
    this.loadActivities();
  }

  loadActivities(): void {
    this.loading = true;
    this.error = null;

    let observable;

    if (this.cardId) {
      observable = this.activityLogService.getCardActivityLogs(this.cardId);
    } else if (this.boardId) {
      observable = this.activityLogService.getBoardActivityLogs(this.boardId, this.page, this.pageSize);
    } else if (this.showUserActivities) {
      observable = this.activityLogService.getMyActivityLogs(this.page, this.pageSize);
    } else {
      this.loading = false;
      return;
    }

    observable.subscribe({
      next: (response: any) => {
        if (Array.isArray(response)) {
          // Card activities response
          this.activities = response;
          this.hasMore = false;
        } else {
          // Board or user activities response with pagination
          if (this.page === 1) {
            this.activities = response.activities;
          } else {
            this.activities = [...this.activities, ...response.activities];
          }
          this.hasMore = response.hasMore;
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading activities:', error);
        this.error = 'Failed to load activities';
        this.loading = false;
      }
    });
  }

  loadMore(): void {
    if (!this.loading && this.hasMore) {
      this.page++;
      this.loadActivities();
    }
  }

  getActivityIcon(activityType: ActivityType): string {
    return this.activityLogService.getActivityIcon(activityType);
  }

  getActivityColor(activityType: ActivityType): string {
    return this.activityLogService.getActivityColor(activityType);
  }

  getRelativeTime(date: Date): string {
    const now = new Date();
    const activityDate = new Date(date);
    const diffMs = now.getTime() - activityDate.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMins / 60);
    const diffDays = Math.floor(diffHours / 24);

    if (diffMins < 1) return 'Just now';
    if (diffMins < 60) return `${diffMins} minute${diffMins > 1 ? 's' : ''} ago`;
    if (diffHours < 24) return `${diffHours} hour${diffHours > 1 ? 's' : ''} ago`;
    if (diffDays < 7) return `${diffDays} day${diffDays > 1 ? 's' : ''} ago`;

    return activityDate.toLocaleDateString();
  }

  getActivityTypeName(activityType: ActivityType): string {
    return ActivityType[activityType].replace(/([A-Z])/g, ' $1').trim();
  }
}
