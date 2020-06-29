import { Component, OnInit, Input } from '@angular/core';
import { Problem } from '../problem';
@Component({
  selector: 'app-preview',
  templateUrl: './preview.component.html',
  styleUrls: ['./preview.component.css']
})
export class PreviewComponent implements OnInit {
  @Input() problem: Problem;
  @Input() showControls = true;

  constructor() { }

  ngOnInit(): void { }
}
