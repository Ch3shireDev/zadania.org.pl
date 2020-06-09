import { Component, OnInit, Input } from '@angular/core';
import { Problem } from '../problem';

@Component({
  selector: 'app-show',
  templateUrl: './show.component.html',
  styleUrls: ['./show.component.css'],
})
export class ShowComponent implements OnInit {
  @Input() problem: Problem;
  @Input() showControls = true;

  constructor() {}

  ngOnInit(): void {
  }
}
