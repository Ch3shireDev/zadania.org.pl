import { Component, OnInit, Input } from '@angular/core';
import { Answer } from '../answer';

@Component({
  selector: 'app-editor',
  templateUrl: './editor.component.html',
  styleUrls: ['./editor.component.css'],
})
export class EditorComponent implements OnInit {
  @Input() answer: Answer;

  constructor() {}

  ngOnInit(): void {}
}
