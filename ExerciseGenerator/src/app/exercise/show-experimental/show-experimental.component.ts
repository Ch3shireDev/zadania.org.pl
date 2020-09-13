import { Component, OnInit, AfterContentInit } from '@angular/core';

@Component({
  selector: 'app-show-experimental',
  templateUrl: './show-experimental.component.html',
  styleUrls: ['./show-experimental.component.css']
})
export class ShowExperimentalComponent implements OnInit, AfterContentInit {

  public variables = [];
  public words = [];

  math = `$x Na + $y O_2 \\rightarrow $z NaO_2`;

  private map = {};

  constructor() { }


  public getVariable(name: string): any {
    return this.variables[this.map[name]];
  }

  public setVariable(name: string, value: any): void {
    this.variables[this.map[name]] = value;
  }

  ngOnInit(): void {
    this.words = this.math.split(' ');
    this.variables = Array(this.words.length);
    this.words.forEach((word: string, i: number) => {
      if (word[0] === '$') {
        this.variables[i] = 5;
        this.map[word.replace('$', '')] = i;
      }
    });
  }

  ngAfterContentInit(): void {
    MathJax.Hub.Queue(['Typeset', MathJax.Hub]);
  }


}
