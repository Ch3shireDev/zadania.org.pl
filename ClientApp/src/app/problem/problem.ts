import { Answer } from './answer/answer';
import { Author } from '../author/author';

export class Problem {
  public id: number;
  public url: string;
  public title: string;
  public content: string;
  public contentHtml: string;
  public source: string;
  public points: number;
  public created: Date;
  public edited: Date;
  public isAnswered: boolean;

  public answers: Answer[];
  public author: Author;
  public tags = [];

  public userUpvoted: boolean;
  public userDownvoted: boolean;

  public authorId: number;
  public authorName: string;
}
