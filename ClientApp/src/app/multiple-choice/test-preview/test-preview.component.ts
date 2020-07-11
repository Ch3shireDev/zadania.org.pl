import { Component, OnInit, Input } from '@angular/core';
import { MultipleChoiceTest } from '../multiple-choice-test';
import { MultipleChoiceService } from '../multiple-choice.service';

@Component({
  selector: 'app-test-preview',
  templateUrl: './test-preview.component.html',
  styleUrls: ['./test-preview.component.css']
})
export class TestPreviewComponent implements OnInit {

  @Input() link: string;
  public test: MultipleChoiceTest;

  constructor(private multipleChoiceService: MultipleChoiceService) { }

  ngOnInit(): void {
    this.multipleChoiceService.getTest(this.link).subscribe(test => { this.test = test; });
  }

}
