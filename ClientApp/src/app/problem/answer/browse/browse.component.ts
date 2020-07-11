import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { Answer } from '../answer';
import { AnswerService } from '../answer.service';
import { ShowState } from '../show-state';

@Component({
  selector: 'app-browse',
  templateUrl: './browse.component.html',
  styleUrls: ['./browse.component.css'],
})
export class BrowseComponent implements OnInit {
  @Input() answers: Answer[];
  @Input() problemId: number;

  @Input() isCreate: boolean;
  @Output() isCreateChange = new EventEmitter<boolean>();
  @Output() reloadChange: EventEmitter<any> = new EventEmitter();

  @Output() state: EventEmitter<ShowState> = new EventEmitter<ShowState>();
  // state: ShowState;

  constructor(private answerService: AnswerService) { }

  ngOnInit(): void { }

  onDelete(event) {
    this.answerService.getAnswers(this.problemId).subscribe((answers) => {
      this.answers = answers;
    });
  }

  changeState(event) {
    this.state.emit(event);
  }

  onReload(event) {
    this.reloadChange.emit(event);
  }

}
