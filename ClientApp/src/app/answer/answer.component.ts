import { Component, OnInit, Input } from '@angular/core';
import { Answer } from './answer';
import { AnswerService } from './answer.service';

@Component({
  selector: 'app-answer',
  templateUrl: './answer.component.html',
  styleUrls: ['./answer.component.css'],
})
export class AnswerComponent implements OnInit {
  @Input() answers: Answer[];
  @Input() parentId: number;
  @Input() isCreate: boolean;

  constructor(private answerService: AnswerService) {}

  ngOnInit(): void {
    console.log(this.answers);
  }

  addAnswer() {
    this.isCreate = true;
  }

  onClose(event) {
    this.isCreate = false;
    if (event) {
      this.answerService.getAnswers(this.parentId).subscribe((answers) => {
        this.answers = answers;
      });
    }
  }

}
