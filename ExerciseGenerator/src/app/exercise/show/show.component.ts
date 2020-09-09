import { ExerciseService } from './../exercise.service';
import { Exercise } from './../exercise';
import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-show',
  templateUrl: './show.component.html',
  styleUrls: ['./show.component.css']
})
export class ShowComponent implements OnInit {

  public id: string;
  public exercise: Exercise;
  public submitted: boolean;
  public content: string;
  public answers: { name, value, userAnswer; }[];
  public result: string;

  constructor(private route: ActivatedRoute, private exerciseService: ExerciseService) {

  }

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');
    this.exerciseService.getExercise(this.id).subscribe(exercise => {
      this.exercise = exercise;
    });
    this.initialize();
  }

  initialize(): void {
    const variables = this.exercise.getVariables();
    this.content = this.exercise.getContent(variables);
    this.answers = this.exercise.getAnswers(variables);
    this.submitted = false;
    this.result = undefined;
  }

  submit(): void {
    this.submitted = true;
    let correct = true;
    let messages = [];
    this.answers.forEach(answer => {
      if (answer.userAnswer === undefined) {
        this.submitted = false;
        messages.push(`Brak wartości ${answer.name}.`);
      }
      if (this.submitted === false) { return; }
      if (answer.value != answer.userAnswer) {
        correct = false;
        messages.push(`Błąd wartości "${answer.name}". Podana wartość: ${answer.userAnswer}, poprawna wartość: ${answer.value}. `);
      }
    });
    if (!correct || !this.submitted) {
      this.result = messages.join('\n');
      return;
    }

    this.result = 'Poprawna odpowiedź!';
    const ex = this;
    setTimeout(() => { ex.next(); }, 1000);
  }

  next(): void {
    if (!this.submitted) { return; }
    this.initialize();
  }

}
