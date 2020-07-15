import { MultipleChoiceQuestion } from './multiple-choice-question';

export class MultipleChoiceTest {
  public id: number;
  public title: string;
  public content: string;
  public contentHtml: string;
  public questions: MultipleChoiceQuestion[];
}
