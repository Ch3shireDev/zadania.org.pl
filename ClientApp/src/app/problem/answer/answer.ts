
export class Answer {
  public id: number;
  public problemId: number;
  public url: string;
  public content: string;
  public contentHtml: string;
  public points: number;
  public authorId: number;
  public authorName: string;
  public userId: string;
  public isApproved: boolean;
  public userUpvoted: boolean;
  public userDownvoted: boolean;
}
