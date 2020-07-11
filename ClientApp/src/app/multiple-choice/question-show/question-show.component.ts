import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { MultipleChoiceQuestion } from '../multiple-choice-question';
import { MultipleChoiceService } from '../multiple-choice.service';

@Component({
  selector: 'app-question-show',
  templateUrl: './question-show.component.html',
  styleUrls: ['./question-show.component.css']
})
export class QuestionShowComponent implements OnInit {

  @Input() link: string;
  public question: MultipleChoiceQuestion;
  @Output() pointsChange: EventEmitter<number> = new EventEmitter<number>();
  answersList: boolean[];
  @Output() answerChange: EventEmitter<number> = new EventEmitter<number>();

  constructor(private multipleChoiceService: MultipleChoiceService) { }

  ngOnInit(): void {
    this.multipleChoiceService.getQuestion(this.link).subscribe(question => {
      this.question = question;
      this.answersList = new Array<boolean>(this.question.answerLinks.length);
    });
  }

  setAnswer(event, i) {
    this.answersList[i] = event;
    let wrong = false;
    let correct = false;
    this.answersList.forEach(x => {
      if (x === false) { wrong = true; }
      if (x === true) { correct = true; }
    });
    if (wrong) {
      this.answerChange.emit(i);
      this.pointsChange.emit(-1); return;
    }
    if (correct) {
      this.answerChange.emit(i);
      this.pointsChange.emit(1); return;
    }
    this.pointsChange.emit(0);
    this.answerChange.emit(null);
  }
}
