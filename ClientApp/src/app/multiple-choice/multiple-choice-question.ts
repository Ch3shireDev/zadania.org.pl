import { MultipleChoiceAnswer } from './multiple-choice-answer';

export class MultipleChoiceQuestion {
  public url: string;
  public content: string;
  public contentHtml: string;
  public solution: string;
  public solutionHtml: string;
  public answers: MultipleChoiceAnswer[];
}
