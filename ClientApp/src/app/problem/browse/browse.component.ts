import { Component, OnInit } from '@angular/core';
import { ProblemService } from '../problem.service';

@Component({
  selector: 'app-browse',
  templateUrl: './browse.component.html',
  styleUrls: ['./browse.component.css'],
})
export class BrowseComponent implements OnInit {
  problems: any[];
  constructor(private problemService: ProblemService) {}

  ngOnInit(): void {
    this.problemService.getProblems().subscribe((res) => {
      this.problems = res;
    });
  }
}
