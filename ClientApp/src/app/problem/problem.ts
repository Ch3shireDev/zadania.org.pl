import { Answer } from '../answer/answer';
import { Author } from '../author/author';

export class Problem {
  public id: number;
  public title: string;
  public content: string;
  public source: string;
  public points: number;
  public created: Date;
  public edited: Date;

  public answers: Answer[];
  public author: Author;
  public tags = [];
}
