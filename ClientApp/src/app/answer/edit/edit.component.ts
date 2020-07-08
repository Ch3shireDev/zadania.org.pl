import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Answer } from '../answer';
import { AnswerService } from '../answer.service';

@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.css'],
})
export class EditComponent implements OnInit {
  @Input() answer: Answer;
  @Input() problemId: number;
  @Output() close: EventEmitter<boolean> = new EventEmitter();

  constructor(private answerService: AnswerService) {}

  ngOnInit(): void {}

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
