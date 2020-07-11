import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { Answer } from '../answer';
import { AnswerService } from '../answer.service';

@Component({
  selector: 'app-answer-create',
  templateUrl: './answer-create.component.html',
  styleUrls: ['./answer-create.component.css'],
})
export class AnswerCreateComponent implements OnInit {
  @Input() problemId: number;
  @Output() close: EventEmitter<boolean> = new EventEmitter();
  answer = new Answer();


  constructor(private answerService: AnswerService) { }

  ngOnInit(): void { }

  onClose() {
    this.close.emit(false);
  }

  submit() {
    this.answerService
      .postAnswer(this.problemId, this.answer)
      .subscribe((res) => {
        this.close.emit(true);
      });
  }
}
