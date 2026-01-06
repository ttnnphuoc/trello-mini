import { Card } from './card';

export interface List {
  id: number;
  title: string;
  position: number;
  boardId: number;
  createdAt: Date;
  updatedAt: Date;
  cards: Card[];
}
