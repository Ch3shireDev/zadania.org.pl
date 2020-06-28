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
  constructor(
    private location: Location,
    private problemService: ProblemService
  ) { }

  ngOnInit(): void { }

  submit() {
    console.log('xxxxxxxx')
    this.problemService.postProblem(this.problem).subscribe((res) => {
      console.log('xxx')
      this.goBack();
    }, err => { console.log(err);console.log('xxxyyyy') });
  }

  goBack() {
    this.location.back();
  }
}
