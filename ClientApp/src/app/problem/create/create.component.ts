import { Component, OnInit } from '@angular/core';
import { Problem } from '../problem';
import { Location } from '@angular/common';
import { ProblemService } from '../problem.service';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css'],
})
export class CreateComponent implements OnInit {
  problem = new Problem();

  errorMessage: string;

  constructor(
    private location: Location,
    private problemService: ProblemService
  ) { }

  ngOnInit(): void { }

  submit() {
    this.errorMessage = null;
    const t = new TurndownService();
    this.problem.content = t.turndown(this.problem.contentHtml);

    this.problemService.postProblem(this.problem).subscribe((res) => {
      this.goBack();
    }, err => {
      if (err.status === 0) { this.errorMessage = 'Błąd połączenia z serwerem.'; }
      else {
        this.errorMessage = err.message;
      }
    });
  }

  goBack() {
    this.location.back();
  }
}
