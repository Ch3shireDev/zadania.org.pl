import { Component, OnInit } from '@angular/core';
import { Problem } from '../problem';
import { ProblemService } from '../problem.service';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';

@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.css'],
})
export class EditComponent implements OnInit {
  problem: Problem;
  id: number;
  constructor(
    private problemService: ProblemService,
    private activatedRoute: ActivatedRoute,
    private location: Location,
  ) { }

  ngOnInit(): void {
    this.id = this.activatedRoute.snapshot.params.id;
    this.problemService.getProblem(this.id).subscribe((problem) => {
      this.problem = problem;
    });
  }

  submit() {
    const t = new TurndownService();
    this.problem.content =  t.turndown(this.problem.contentHtml);
    this.problemService.putProblem(this.id, this.problem).subscribe((res) => {
      this.goBack();
    });
  }

  goBack() {
    this.location.back();
  }
}
