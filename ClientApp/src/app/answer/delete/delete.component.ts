import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Answer } from '../answer';
import { AnswerService } from '../answer.service';

@Component({
  selector: 'app-delete',
  templateUrl: './delete.component.html',
  styleUrls: ['./delete.component.css'],
})
export class DeleteComponent implements OnInit {
  @Input() answer: Answer;
  @Input() parentId: number;
  @Output() close: EventEmitter<boolean> = new EventEmitter();

  constructor(private answerService: AnswerService) {}

  ngOnInit(): void {}

  submit() {
    this.answerService
      .deleteAnswer(this.parentId, this.answer.id)
      .subscribe((res) => {
        this.close.emit(true);
      });
  }

  goBack() {
    this.close.emit(false);
  }
}
