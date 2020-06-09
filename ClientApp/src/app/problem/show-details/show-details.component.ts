import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProblemService } from '../problem.service';
import { Problem } from '../problem';
import { Location } from '@angular/common';

@Component({
  selector: 'app-show-details',
  templateUrl: './show-details.component.html',
  styleUrls: ['./show-details.component.css'],
})
export class ShowDetailsComponent implements OnInit {
  id: number;
  public problem: Problem;
  constructor(
    private activatedRoute: ActivatedRoute,
    private problemService: ProblemService,
    private location: Location
  ) {}

  ngOnInit(): void {
    this.id = this.activatedRoute.snapshot.params.id;
    this.problemService.getProblem(this.id).subscribe((problem) => {
      this.problem = problem;
      console.log(this.problem);
    });
  }

  goBack() {
    this.location.back();
  }

  upvote() {
    this.problemService.upvoteProblem(this.id).subscribe((res) => {
      this.problem.points = res['points'];
    });
  }

  downvote() {
    this.problemService.downvoteProblem(this.id).subscribe((res) => {
      this.problem.points = res['points'];
    });
  }
}
