import { Board } from './board';
import { List } from './list';
import { Card } from './card';
import { User } from './auth';

export enum ActivityType {
  // Board activities
  BoardCreated = 0,
  BoardUpdated = 1,
  BoardDeleted = 2,
  BoardMemberAdded = 3,
  BoardMemberRemoved = 4,
  BoardMemberRoleChanged = 5,

  // List activities
  ListCreated = 6,
  ListUpdated = 7,
  ListDeleted = 8,
  ListMoved = 9,

  // Card activities
  CardCreated = 10,
  CardUpdated = 11,
  CardDeleted = 12,
  CardMoved = 13,
  CardPriorityChanged = 14,
  CardDueDateChanged = 15,
  CardAssigned = 16,
  CardUnassigned = 17,

  // Collaboration activities
  CommentAdded = 18,
  CommentUpdated = 19,
  CommentDeleted = 20,
  AttachmentAdded = 21,
  AttachmentDeleted = 22,

  // User activities
  UserJoined = 23,
  UserLeft = 24
}

export interface ActivityLog {
  id: number;
  activityType: ActivityType;
  description: string;
  data?: string;
  createdAt: Date;
  userId?: number;
  user?: User;
  boardId?: number;
  board?: Board;
  listId?: number;
  list?: List;
  cardId?: number;
  card?: Card;
  ipAddress?: string;
  userAgent?: string;
}

export interface ActivityLogResponse {
  activities: ActivityLog[];
  totalCount: number;
  page: number;
  pageSize: number;
  hasMore: boolean;
}
