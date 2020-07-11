import { Component, OnInit, Pipe } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProblemService } from '../problem.service';
import { Problem } from '../problem';
import { Location } from '@angular/common';
import { SafeHtml } from '@angular/platform-browser';
import { AuthService } from 'src/app/auth.service';


@Component({
  selector: 'app-show',
  templateUrl: './show.component.html',
  styleUrls: ['./show.component.css'],
})
export class ShowComponent implements OnInit {
  id: number;
  public problem: Problem;
  sanitized: SafeHtml;
  isAuthor = false;
  constructor(
    private activatedRoute: ActivatedRoute,
    private problemService: ProblemService,
    public authService: AuthService
  ) { }

  ngOnInit(): void {
    this.id = this.activatedRoute.snapshot.params.id;
    this.reload();
  }

  goBack() {
    window.history.back();
    // this.location.back();
  }


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

  reload() {
    this.problemService.getProblem(this.id).subscribe((problem) => {
      this.problem = problem;
      this.problem.edited = new Date(this.problem.edited);
      this.authService.userProfile$.subscribe(profile => {
        if (profile == null) {
          this.isAuthor = false;
        }
        else {
          this.isAuthor = problem.authorId === profile.sub;
        }
      });
    });
  }
}
