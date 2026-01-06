export interface Card {
  id: number;
  title: string;
  description?: string;
  position: number;
  dueDate?: Date;
  listId: number;
  createdAt: Date;
  updatedAt: Date;
}

export interface MoveCardRequest {
  listId: number;
  position: number;
}
