import { ExerciseService } from './../exercise.service';
import { Component, OnInit } from '@angular/core';
import { Exercise } from '../exercise';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css']
})
export class CreateComponent implements OnInit {

  public exercise: Exercise = new Exercise();

  constructor(private exerciseService: ExerciseService, private router: Router) { }

  ngOnInit(): void {
  }

  save(): void {
    this.exerciseService.createExercise(this.exercise).subscribe(res => {
      this.router.navigate([res.id]);
    });
  }
}
