export enum CardPriority {
  None = 0,
  Low = 1,
  Medium = 2,
  High = 3,
  Critical = 4
}

export interface Card {
  id: number;
  title: string;
  description?: string;
  position: number;
  dueDate?: Date;
  priority: CardPriority;
  listId: number;
  createdAt: Date;
  updatedAt: Date;
}

export interface MoveCardRequest {
  listId: number;
  position: number;
}
