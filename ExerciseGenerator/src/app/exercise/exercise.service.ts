import { Injectable } from '@angular/core';
import { Exercise, DataType } from './exercise';
import { Observable, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ExerciseService {

  get exercises(): Exercise[] {

    let exerciseData = [];

    try {
      // const exerciseStr = localStorage.getItem('exercises');
      // exerciseData = JSON.parse(exerciseStr);
      throw new Error();
    }
    catch {
      exerciseData = [{
        id: '1',
        title: 'Dynamika ruchu postępowego',
        content: 'Winda może poruszać się w górę i w dół z przyspieszeniem o takiej samej wartości. W windzie tej na wadze sprężynowej stoi studentka. Różnica wskazań wagi przy ruchu w górę i w dół wynosi $x N. Jakie jest przyspieszenie windy, jeżeli ciężar studentki wynosi $y N?',
        variableData: [
          { name: 'x', expression: 'Math.floor(Math.random() * 10+1)*10', type: DataType.Int },
          { name: 'y', expression: 'Math.floor(Math.random() * 3)*100+300', type: DataType.Int }
        ],
        answerData: [
          { name: 'z', description: 'Przyspieszenie windy', expression: '$x / $y / 2', type: DataType.Double }
        ],
      }];
      console.log(exerciseData);
    }

    const array = exerciseData.map(e => Object.assign(new Exercise(), e));
    return array;
  }

  set exercises(value: Exercise[]) {
    localStorage.setItem('exercises', JSON.stringify(value));
  }


  constructor() { }

  deleteExercise(id: string): Observable<any> {

    const exercises = [];

    this.exercises.forEach(e => {
      if (e.id === id) { return; }
      exercises.push(e);
    });

    this.exercises = exercises;

    return new Observable();
  }

  updateExercise(exercise: Exercise): Observable<any> {
    let index = -1;
    const exercises = this.exercises;
    exercises.forEach((e, i) => {
      if (e.id === exercise.id) {
        index = i;
      }
    });
    if (index === -1) { return; }
    exercises[index] = exercise;
    this.exercises = exercises;
    return new Observable();
  }

  createExercise(exercise: Exercise): Observable<{ id; }> {
    exercise.id = this.exercises.length.toString();
    const exercises = this.exercises;
    exercises.push(exercise);
    this.exercises = exercises;
    const observer = new Observable<{ id; }>(obs => {
      obs.next({ id: exercise.id });
      obs.complete();
    });
    return observer;
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
