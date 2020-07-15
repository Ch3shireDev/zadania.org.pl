import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { MultipleChoiceAnswer } from '../multiple-choice-answer';
import { MultipleChoiceService } from '../multiple-choice.service';

@Component({
  selector: 'app-answer-show',
  templateUrl: './answer-show.component.html',
  styleUrls: ['./answer-show.component.css']
})
export class AnswerShowComponent implements OnInit {
  @Input() answer: MultipleChoiceAnswer;
  @Input() checked = false;
  @Input() readOnly = false;
  @Output() setAnswer: EventEmitter<boolean> = new EventEmitter<boolean>();
  @ViewChild('inputAnswer') input;

  public id: string;

  constructor(private multipleChoiceService: MultipleChoiceService) { }

  ngOnInit(): void {
    if (!this.answer) { return; }
    if (!this.answer.content) { return; }
    this.id = this.answer.url.split('/')[6];
    this.multipleChoiceService.getAnswer(this.answer.url).subscribe(answer => { this.answer = answer; });
  }

  checkAnswer() {
    this.answer.isChecked = this.input.nativeElement.checked;

    if (this.answer.isChecked) {
      this.answer.isChecked = false;
      this.input.nativeElement.checked = false;
      this.setAnswer.emit(null);
      return;
    }
    this.answer.isChecked = true;
    this.input.nativeElement.checked = true;
    if (this.answer.isCorrect) {
      this.setAnswer.emit(true);
      return;
    }
    this.setAnswer.emit(false);
  }

}
