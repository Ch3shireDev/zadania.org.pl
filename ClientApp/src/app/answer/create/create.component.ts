import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { Answer } from '../answer';
import { AnswerService } from '../answer.service';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css'],
})
export class CreateComponent implements OnInit {
  @Input() parentId: number;
  @Output() close: EventEmitter<boolean> = new EventEmitter();
  answer = new Answer();


  constructor(private answerService: AnswerService) {}

  ngOnInit(): void {}

  onClose() {
    this.close.emit(false);
  }

  submit() {
    this.answerService
      .postAnswer(this.parentId, this.answer)
      .subscribe((res) => {
        this.close.emit(true);
      });
  }
}
