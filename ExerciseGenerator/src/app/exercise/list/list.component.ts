import { ExerciseService } from './../exercise.service';

import { Component, OnInit, Input } from '@angular/core';
import { Exercise } from '../exercise';
// import { exercises } from '../exercises';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.css']
})
export class ListComponent implements OnInit {

  public exercises: { id, title; }[];

  constructor(private exerciseService: ExerciseService) {

  }

  ngOnInit(): void {
    this.exerciseService.getExercises().subscribe(exercises => {
      this.exercises = exercises;
    });
  }

}
