import { Component, OnInit, Input } from '@angular/core';
import { Answer } from '../answer';
import { AnswerService } from '../answer.service';

@Component({
  selector: 'app-browse',
  templateUrl: './browse.component.html',
  styleUrls: ['./browse.component.css'],
})
export class BrowseComponent implements OnInit {
  @Input() answers: Answer[];
  @Input() parentId: number;

  constructor(private answerService: AnswerService) {}

  ngOnInit(): void {}

  onDelete(event) {
    this.answerService.getAnswers(this.parentId).subscribe((answers) => {
      this.answers = answers;
    });
  }
}
