import { Author } from '../author/author';

export class Answer {
  public id: number;
  public problemId: number;
  public content: string;
  public contentHtml: string;
  public points: number;
  public author: Author;
  public isApproved: boolean;
  public userUpvoted: boolean;
  public userDownvoted: boolean;
}
