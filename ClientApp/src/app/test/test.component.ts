import { Component, OnInit } from '@angular/core';
import { TestService } from '../test.service';

@Component({
  selector: 'app-test',
  templateUrl: './test.component.html',
  styleUrls: ['./test.component.css'],
})
export class TestComponent implements OnInit {
  result: string;
  constructor(private testService: TestService) {}

  ngOnInit(): void {}

  public() {
    this.testService.public().subscribe((res) => {
      this.result = res;
    });
  }
  private() {
    this.testService.private().subscribe((res) => {
      this.result = res;
    });
  }
}
