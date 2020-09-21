import { Component, OnInit, Input } from '@angular/core';
import { Exercise, DataType as DataType } from '../exercise';

@Component({
  selector: 'app-editor',
  templateUrl: './editor.component.html',
  styleUrls: ['./editor.component.css']
})
export class EditorComponent implements OnInit {

  public Object = Object;

  @Input() exercise: Exercise;

  constructor() {
  }

  ngOnInit(): void {
  }

  addVariable(): void {
    this.exercise.variableData.push({ name: 'new_var', description: 'Zmienna', expression: '123', type: DataType.Int });
  }

  removeVariable(): void {
    this.exercise.variableData.pop();
  }

  addQuestion(): void {
    this.exercise.answerData.push({ name: 'z', description: 'Rozwiązanie', expression: '$x + $y', type: DataType.Int });
  }

  removeQuestion(): void {
    this.exercise.answerData.pop();
  }

}
