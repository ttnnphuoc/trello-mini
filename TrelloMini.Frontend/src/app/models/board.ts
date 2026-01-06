import { List } from './list';

export interface Board {
  id: number;
  title: string;
  description?: string;
  createdAt: Date;
  updatedAt: Date;
  lists: List[];
}
