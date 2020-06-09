import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Problem } from '../problem';
import { Location } from '@angular/common';

@Component({
  selector: 'app-editor',
  templateUrl: './editor.component.html',
  styleUrls: ['./editor.component.css'],
})
export class EditorComponent implements OnInit {
  @Input() problem = new Problem();
  @Input() title: string;
  // @Output() submit: EventEmitter<Problem> = new EventEmitter<Problem>();

  constructor() {}

  ngOnInit(): void {}

  // onSubmit() {
  //   this.submit.emit(this.problem);
  // }

  // goBack() {
  //   this.location.back();
  // }
}
