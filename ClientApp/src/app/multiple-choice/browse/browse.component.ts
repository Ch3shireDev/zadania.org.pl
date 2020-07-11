import { Component, OnInit } from '@angular/core';
import { MultipleChoiceService } from '../multiple-choice.service';

@Component({
  selector: 'app-browse',
  templateUrl: './browse.component.html',
  styleUrls: ['./browse.component.css']
})
export class BrowseComponent implements OnInit {

  public links: string[];

  constructor(private multipleChoiceService: MultipleChoiceService) { }

  ngOnInit(): void {
    this.multipleChoiceService.getTests().subscribe(tests => {
      this.links = tests['multipleChoiceTestLinks'];
    })
  }

}
