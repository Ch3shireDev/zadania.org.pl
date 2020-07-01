import { Author } from '../author/author';

export class Answer {
  public id: number;
  public parentId: number;
  public content: string;
  public points: number;
  public author: Author;
  public isApproved: boolean;
  public userUpvoted: boolean;
  public userDownvoted: boolean;
}
