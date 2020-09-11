import { ExerciseService } from './../exercise.service';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Exercise } from '../exercise';

@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.css']
})
export class EditComponent implements OnInit {

  public id: string;
  public exercise: Exercise;
  public Object = Object;

  constructor(private route: ActivatedRoute, private exerciseService: ExerciseService, private router: Router) {
  }

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');
    this.exerciseService.getExercise(this.id).subscribe(exercise => {
      this.exercise = exercise;
    });
  }

  save(): void {
    this.exerciseService.updateExercise(this.exercise);
    this.router.navigate([this.id]);
  }

}
