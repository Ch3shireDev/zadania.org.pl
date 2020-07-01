import { Component, OnInit, Input } from '@angular/core';
import { Problem } from '../problem';
import { ProblemService } from '../problem.service';
import { AuthService } from 'src/app/auth.service';
@Component({
  selector: 'app-preview',
  templateUrl: './preview.component.html',
  styleUrls: ['./preview.component.css']
})
export class PreviewComponent implements OnInit {
  @Input() problem: Problem;
  @Input() showControls = true;

  constructor(private problemService: ProblemService, public authService: AuthService) { }

  ngOnInit(): void { }

  upvote(event) {
    event.stopPropagation();
    if (this.problem.userDownvoted) {
      this.problem.points += 1;
    }
    this.problem.userDownvoted = false;
    this.problem.userUpvoted = !this.problem.userUpvoted;
    if (this.problem.userUpvoted) {
      this.problem.points += 1;
    }
    else {
      this.problem.points -= 1;
    }
    this.problemService.upvoteProblem(this.problem).subscribe(res => { });
  }

  downvote(event) {
    event.stopPropagation();
    if (this.problem.userUpvoted) {
      this.problem.points -= 1;
    }
    this.problem.userUpvoted = false;
    this.problem.userDownvoted = !this.problem.userDownvoted;
    if (this.problem.userDownvoted) {
      this.problem.points -= 1;
    }
    else {
      this.problem.points += 1;
    }
    this.problemService.downvoteProblem(this.problem).subscribe(res => { });
  }

}
