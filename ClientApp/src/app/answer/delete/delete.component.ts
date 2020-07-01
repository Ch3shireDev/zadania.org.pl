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
  @Output() closeChange: EventEmitter<boolean> = new EventEmitter();

  @Input() isCreate: boolean;
  @Output() isCreateChange = new EventEmitter<boolean>();

  constructor(private answerService: AnswerService) { }

  ngOnInit(): void { }

  submit() {
    this.answerService
      .deleteAnswer(this.parentId, this.answer.id)
      .subscribe((res) => {
        this.closeChange.emit(true);
      });
  }

  goBack() {
    this.closeChange.emit(false);
  }
}
