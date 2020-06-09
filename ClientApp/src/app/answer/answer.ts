import { Author } from '../author/author';

export class Answer {
  public id: number;
  public content: string;
  public points: number;
  public author: Author;
}
