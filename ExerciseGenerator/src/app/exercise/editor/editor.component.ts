import { Component, OnInit, Input } from '@angular/core';
import { Exercise, DataType as DataType } from '../exercise';

@Component({
  selector: 'app-editor',
  templateUrl: './editor.component.html',
  styleUrls: ['./editor.component.css']
})
export class EditorComponent implements OnInit {

  // public id: string;
  public Object = Object;
  // public newVariable = false;

  // public newKey: string;
  // public newData: string;

  // public alertMessage: string;
  // public successMessage: string;

  // public example: string;

  @Input() exercise: Exercise;

  constructor() {
  }

  ngOnInit(): void {
  }

  addVariable(): void {
    this.exercise.variableData.push({ name: 'new_var', expression: '123', type: DataType.Int });
  }

  removeVariable(): void {
    this.exercise.variableData.pop();
  }

  addQuestion(): void {
    this.exercise.answerData.push({ name: 'Rozwiązanie', expression: '$x + $y', type: DataType.Int });
  }

  removeQuestion(): void {
    this.exercise.answerData.pop();
  }

  // addVariable(): void {
  //   this.newVariable = true;
  //   this.newKey = '';
  //   this.newData = null;
  // }

  // submitVariable(): void {
  //   if (this.evaluate()) {
  //     //
  //   }
  // }

  // resetAlert(): void {
  //   this.alertMessage = '';
  // }

  // evaluate(): boolean {
  //   this.newKey = this.newKey.trim();
  //   if (this.newKey in this.exercise.variableData) {
  //     this.alertMessage = `Zmienna ${this.newKey} już znajduje się w bazie!`;
  //     return;
  //   }
  //   const result = this.exercise.evaluate(this.newData, this.exercise.getVariables());

  //   if (result === undefined) {
  //     this.alertMessage = 'Błąd kompilacji skryptu!';
  //     return;
  //   }
  //   this.successMessage = 'Wszystko ok!';
  // }

  // generate(): void {
  //   // this.example = this.exercise.generate();
  // }

  // saveIdentifier(): void {

  // }

}
