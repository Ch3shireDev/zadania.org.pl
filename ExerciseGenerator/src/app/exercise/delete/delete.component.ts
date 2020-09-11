import { ActivatedRoute, Router } from '@angular/router';
import { ExerciseService } from './../exercise.service';
import { Component, OnInit } from '@angular/core';
import { Exercise } from '../exercise';

@Component({
  selector: 'app-delete',
  templateUrl: './delete.component.html',
  styleUrls: ['./delete.component.css']
})
export class DeleteComponent implements OnInit {

  public id: string;
  public exercise: Exercise;
  public content: string;

  constructor(private exerciseService: ExerciseService, private route: ActivatedRoute, private router: Router) { }

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');
    this.exerciseService.getExercise(this.id).subscribe(e => {
      this.exercise = e;
      this.content = this.exercise.getContent();
    });
  }

  delete(): void {
    this.exerciseService.deleteExercise(this.id).subscribe();
    this.router.navigateByUrl('/');
  }

}
