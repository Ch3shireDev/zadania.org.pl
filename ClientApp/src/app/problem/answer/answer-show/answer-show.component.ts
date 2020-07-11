import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Answer } from '../answer';
import { AnswerService } from '../answer.service';
import { ShowState } from '../show-state';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/auth.service';

@Component({
  selector: 'app-answer-show',
  templateUrl: './answer-show.component.html',
  styleUrls: ['./answer-show.component.css'],
})
export class AnswerShowComponent implements OnInit {
  public answer: Answer;
  @Input() problemId: number;
  @Input() link: string;

  @Input() isCreate: boolean;
  @Output() isCreateChange = new EventEmitter<boolean>();
  @Output() reloadChange: EventEmitter<any> = new EventEmitter();
  @Output() delete: EventEmitter<boolean> = new EventEmitter();
  @Output() stateChange = new EventEmitter<ShowState>();

  sub: string;
  state = ShowState.Show;
  showState = ShowState;

  constructor(private answerService: AnswerService, private router: Router, public authService: AuthService) { }

  ngOnInit(): void {
    this.answerService.getAnswerFromLink(this.link).subscribe(answer => { this.answer = answer; });


    this.authService.userProfile$.subscribe(profile => {
      if (profile !== null) {
        this.sub = profile.sub;
      }
    });
  }

  onEdit() {
    this.state = ShowState.Edit;
    this.stateChange.emit(this.state);
  }

  onEditClose(event) {
    if (event === true) {
      this.answerService
        .getAnswer(this.problemId, this.answer.id)
        .subscribe((answer) => {
          this.answer = answer;
          this.state = ShowState.Show;
          this.stateChange.emit(this.state);
        });
    } else {
      this.state = this.showState.Show;
      this.stateChange.emit(this.state);
    }
  }

  onDelete() {
    this.state = ShowState.Delete;
    this.stateChange.emit(this.state);
  }

  onDeleteClose(event) {
    this.delete.emit(event);
  }

  upvote(event) {
    if (this.answer.userDownvoted) {
      this.answer.points += 1;
    }
    this.answer.userDownvoted = false;
    this.answer.userUpvoted = !this.answer.userUpvoted;
    if (this.answer.userUpvoted) {
      this.answer.points += 1;
    }
    else {
      this.answer.points -= 1;
    }
    this.answerService.upvoteAnswer(this.answer).subscribe(res => { });
  }

  downvote(event) {
    if (this.answer.userUpvoted) {
      this.answer.points -= 1;
    }
    this.answer.userUpvoted = false;
    this.answer.userDownvoted = !this.answer.userDownvoted;
    if (this.answer.userDownvoted) {
      this.answer.points -= 1;
    }
    else {
      this.answer.points += 1;
    }
    this.answerService.downvoteAnswer(this.answer).subscribe(res => { });
  }
  approveAnswer() {
    this.answerService.approveAnswer(this.answer).subscribe(res => { this.reloadComponent(); });
  }

  disapproveAnswer() {
    this.answerService.disapproveAnswer(this.answer).subscribe(res => { this.reloadComponent(); });
  }

  reloadComponent() {
    this.reloadChange.emit(true);
    // this.router.routeReuseStrategy.shouldReuseRoute = () => false;
    // this.router.onSameUrlNavigation = 'reload';
    // this.router.navigate([`/problems/${this.answer.problemId}`]);
  }

}
