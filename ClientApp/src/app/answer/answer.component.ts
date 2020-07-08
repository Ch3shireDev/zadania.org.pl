import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { Answer } from './answer';
import { AnswerService } from './answer.service';
import { ShowState } from './show-state';
import { AuthService } from '../auth.service';
import { Location } from '@angular/common';

@Component({
  selector: 'app-answer',
  templateUrl: './answer.component.html',
  styleUrls: ['./answer.component.css'],
})
export class AnswerComponent implements OnInit {
  @Input() answers: Answer[];
  @Input() problemId: number;
  @Input() isCreate: boolean;
  state = ShowState.Show;

  public ShowStates = ShowState;
  @Output() reloadChange: EventEmitter<any> = new EventEmitter();

  constructor(private answerService: AnswerService, private authService: AuthService, private location: Location) { }

  changeState(event) {
    this.state = event;
  }

  ngOnInit(): void {
    console.log(this.answers);
  }

  addAnswer() {
    this.authService.isAuthenticated$.subscribe(res => {
      if (res) {

        this.state = ShowState.Create;
        this.isCreate = true;
      }
      else {
        this.authService.login(this.location.path());
      }
    });
  }

  onClose(event) {
    this.state = ShowState.Show;
    this.isCreate = false;
    if (event) {
      this.answerService.getAnswers(this.problemId).subscribe((answers) => {
        this.answers = answers;
      });
    }
  }

  onReload(event) {
    this.reloadChange.emit(event);
  }
}
