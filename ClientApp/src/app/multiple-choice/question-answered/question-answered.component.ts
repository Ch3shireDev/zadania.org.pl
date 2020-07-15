import { Component, OnInit, Input } from '@angular/core';
import { MultipleChoiceQuestion } from '../multiple-choice-question';
import { MultipleChoiceService } from '../multiple-choice.service';

@Component({
  selector: 'app-question-answered',
  templateUrl: './question-answered.component.html',
  styleUrls: ['./question-answered.component.css']
})
export class QuestionAnsweredComponent implements OnInit {

  @Input() answered: number;

  @Input()
  public question: MultipleChoiceQuestion;

  constructor(private multipleChoiceService: MultipleChoiceService) { }

  ngOnInit(): void {
    this.multipleChoiceService.getQuestion(this.question.url).subscribe(question => { this.question = question; });
  }

}
