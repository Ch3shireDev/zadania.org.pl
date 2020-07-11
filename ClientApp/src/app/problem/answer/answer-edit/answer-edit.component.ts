import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Answer } from '../answer';
import { AnswerService } from '../answer.service';

@Component({
  selector: 'app-answer-edit',
  templateUrl: './answer-edit.component.html',
  styleUrls: ['./answer-edit.component.css'],
})
export class AnswerEditComponent implements OnInit {
  @Input() answer: Answer;
  @Input() problemId: number;
  @Output() close: EventEmitter<boolean> = new EventEmitter();

  constructor(private answerService: AnswerService) { }

  ngOnInit(): void { }

  submit() {
    this.answerService
      .putAnswer(this.problemId, this.answer.id, this.answer)
      .subscribe((res) => {
        this.close.emit(true);
      });
  }

  goBack() {
    this.close.emit(false);
  }
}
