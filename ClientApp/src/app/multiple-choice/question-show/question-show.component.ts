import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { MultipleChoiceQuestion } from '../multiple-choice-question';
import { MultipleChoiceService } from '../multiple-choice.service';

@Component({
  selector: 'app-question-show',
  templateUrl: './question-show.component.html',
  styleUrls: ['./question-show.component.css']
})
export class QuestionShowComponent implements OnInit {

  @Input() public question: MultipleChoiceQuestion;
  @Output() pointsChange: EventEmitter<number> = new EventEmitter<number>();
  answersList: boolean[];
  @Output() answerChange: EventEmitter<number> = new EventEmitter<number>();

  constructor(private multipleChoiceService: MultipleChoiceService) { }

  ngOnInit(): void {
    console.log(this.question);
    if (this.question.content != null) {
      this.answersList = new Array<boolean>(this.question.answers.length);
      return;
    }
    this.multipleChoiceService.getQuestion(this.question.url).subscribe(question => {
      this.question = question;
      this.answersList = new Array<boolean>(this.question.answers.length);
    });
  }

  setAnswer(event, i) {

    if (event === false) {
      this.answerChange.emit(i);
      this.pointsChange.emit(-1); return;
    }
    if (event === true) {
      this.answerChange.emit(i);
      this.pointsChange.emit(1); return;
    }
    if (event === null) {
      this.pointsChange.emit(0);
      this.answerChange.emit(null);
    }
  }
}
