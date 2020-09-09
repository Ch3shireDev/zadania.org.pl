import { Component, OnInit } from '@angular/core';
import { Exercise } from './exercise';

@Component({
  selector: 'app-exercise',
  templateUrl: './exercise.component.html',
  styleUrls: ['./exercise.component.css']
})
export class ExerciseComponent implements OnInit {

  public exercise: Exercise = new Exercise();

  public exercises: Exercise[];

  public ngOnInit(): void {
    this.exercises = [this.exercise];
  }

}
