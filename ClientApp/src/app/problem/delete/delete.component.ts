import { Component, OnInit } from '@angular/core';
import { ProblemService } from '../problem.service';
import { Problem } from '../problem';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { Location } from '@angular/common';

@Component({
  selector: 'app-delete',
  templateUrl: './delete.component.html',
  styleUrls: ['./delete.component.css'],
})
export class DeleteComponent implements OnInit {
  id: number;
  problem: Problem;
  problemLink: string;

  constructor(
    private problemService: ProblemService,
    private activatedRoute: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.id = this.activatedRoute.snapshot.params.id;
    this.problemLink = `/api/v1/problems/${this.id}`;
    this.problemService.getProblem(this.id).subscribe((problem) => {
      this.problem = problem;
    });
  }

  confirmDelete() {
    this.problemService.deleteProblem(this.id).subscribe((res) => {
      this.goBack();
    });
  }

  goBack() {
    this.router.navigateByUrl('/problems');
  }
}
