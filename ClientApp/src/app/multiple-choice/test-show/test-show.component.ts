import { Component, OnInit } from '@angular/core';
import { MultipleChoiceTest } from '../multiple-choice-test';
import { MultipleChoiceService } from '../multiple-choice.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-test-show',
  templateUrl: './test-show.component.html',
  styleUrls: ['./test-show.component.css']
})
export class TestShowComponent implements OnInit {

  public test: MultipleChoiceTest;
  public id: string;
  public currentIndex: number;
  public maxIndex: number;

  public step = 1;
  public points: number[];
  public answers: number[];

  public numPoints: number;

  constructor(private multipleChoiceService: MultipleChoiceService, private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.id = this.activatedRoute.snapshot.paramMap.get('id');
    this.multipleChoiceService.getTestById(this.id).subscribe(test => {
      this.test = test;
      this.currentIndex = 0;
      this.maxIndex = test.questionLinks.length;
      this.points = new Array(this.maxIndex);
      this.answers = new Array(this.maxIndex);
    });
  }

  startTest(): void {
    this.step = 2;
  }

  questionPrev() {
    this.currentIndex -= 1;
  }

  questionNext() {
    this.currentIndex += 1;
  }

  submit() {
    this.step = 3;
    this.numPoints = this.points.reduce((acc, cur) => acc + cur, 0);
  }

  setPoints(event, i) {
    this.points[i] = event;
  }

  setAnswer(event, i) {
    this.answers[i] = event;
  }

}
