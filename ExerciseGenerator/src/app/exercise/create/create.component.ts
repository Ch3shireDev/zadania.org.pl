import { ExerciseService } from './../exercise.service';
import { Component, OnInit } from '@angular/core';
import { Exercise } from '../exercise';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css']
})
export class CreateComponent implements OnInit {

  public exercise: Exercise = new Exercise();

  constructor(private exerciseService: ExerciseService) { }

  ngOnInit(): void {
  }

  save(): void {
    this.exerciseService.createExercise(this.exercise);
  }
}
