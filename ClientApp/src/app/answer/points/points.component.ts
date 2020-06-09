import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { faAngleUp, faAngleDown } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-points',
  templateUrl: './points.component.html',
  styleUrls: ['./points.component.css'],
})
export class PointsComponent implements OnInit {
  @Input() element;
  faAngleUp = faAngleUp;
  faAngleDown = faAngleDown;
  @Output() upvote: EventEmitter<boolean> = new EventEmitter();
  @Output() downvote: EventEmitter<boolean> = new EventEmitter();

  constructor() {}

  ngOnInit(): void {}

  onUpvote() {
    this.upvote.emit();
  }

  onDownvote() {
    this.downvote.emit();
  }
}
