import { Injectable } from '@angular/core';
import { Exercise, DataType } from './exercise';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ExerciseService {
  exercises = [
    Object.assign(new Exercise(), {
      id: '1',
      title: 'Zadanie z dodawania',
      content: '$x + $y jest równe:',
      variableData: [
        { name: 'x', expression: 'Math.floor(Math.random() * 10)', type: DataType.Int },
        { name: 'y', expression: 'Math.floor(Math.random() * 10)', type: DataType.Int }
      ],
      answerData: [
        { name: 'Rozwiązanie', expression: '$x + $y', type: DataType.Int }
      ],
    })
  ];

  constructor() { }

  updateExercise(exercise: Exercise): void {
    let index = -1;
    this.exercises.forEach((e, i) => {
      if (e.id === exercise.id) {
        index = i;
      }
    });
    if (index === -1) { return; }
    this.exercises[index] = exercise;
  }

  createExercise(exercise: Exercise): void {
    exercise.id = this.exercises.length.toString();
    this.exercises.push(exercise);
  }

  public getExercises(): Observable<{ id, title; }[]> {

    const obs = new Observable<{ id, title; }[]>(
      observer => {
        const array = [];
        this.exercises.forEach(exercise => {
          array.push({ id: exercise.id, title: exercise.title });
        });

        observer.next(array);
        observer.complete();
      }

    );

    return obs;

  }

  public getExercise(id: string): Observable<Exercise> {
    const obs = new Observable<Exercise>(
      observer => {
        this.exercises.forEach(exercise => {
          if (exercise.id === id) {
            observer.next(exercise);
          }
        });
        observer.complete();
      }
    );
    return obs;
  }

}
