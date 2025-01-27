import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Answer } from '../answer';
import { AnswerService } from '../answer.service';

@Component({
  selector: 'app-answer-delete',
  templateUrl: './answer-delete.component.html',
  styleUrls: ['./answer-delete.component.css'],
})
export class AnswerDeleteComponent implements OnInit {
  @Input() answer: Answer;
  @Input() problemId: number;
  @Output() closeChange: EventEmitter<boolean> = new EventEmitter();

  @Input() isCreate: boolean;
  @Output() isCreateChange = new EventEmitter<boolean>();

  constructor(private answerService: AnswerService) { }

  ngOnInit(): void { }

  submit() {
    this.answerService
      .deleteAnswer(this.problemId, this.answer.id)
      .subscribe((res) => {
        this.closeChange.emit(true);
      });
  }

  goBack() {
    this.closeChange.emit(false);
  }
}
