import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Answer } from '../answer';
import { AnswerService } from '../answer.service';

enum ShowState {
  Show,
  Edit,
  Delete,
}

@Component({
  selector: 'app-show',
  templateUrl: './show.component.html',
  styleUrls: ['./show.component.css'],
})
export class ShowComponent implements OnInit {
  @Input() answer: Answer;
  @Input() parentId: number;
  state = ShowState.Show;
  showState = ShowState;
  @Output() delete: EventEmitter<boolean> = new EventEmitter();

  constructor(private answerService: AnswerService) {}

  ngOnInit(): void {}

  onEdit() {
    this.state = ShowState.Edit;
  }

  onEditClose(event) {
    if (event === true) {
      this.answerService
        .getAnswer(this.parentId, this.answer.id)
        .subscribe((answer) => {
          this.answer = answer;
          this.state = ShowState.Show;
        });
    } else {
      this.state = this.showState.Show;
    }
  }

  onDelete() {
    this.state = ShowState.Delete;
  }

  onDeleteClose(event) {
    this.delete.emit(event);
  }

  onUpvote() {
    this.answerService
      .upvoteAnswer(this.parentId, this.answer.id)
      .subscribe((res) => {
        this.answer.points = res['points'];
      });
  }

  onDownvote() {
    this.answerService
      .downvoteAnswer(this.parentId, this.answer.id)
      .subscribe((res) => {
        this.answer.points = res['points'];
      });
  }
}
