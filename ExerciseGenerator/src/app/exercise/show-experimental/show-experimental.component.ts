import { Component, OnInit, AfterContentInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';

@Component({
  selector: 'app-show-experimental',
  templateUrl: './show-experimental.component.html',
  styleUrls: ['./show-experimental.component.css']
})
export class ShowExperimentalComponent implements OnInit, AfterContentInit {

  public variables = [];
  public words = [];

  math = `Oblicz współczynniki dla poniższego równania: $$ \\ce{SO4^2- +} $x \\ce{Ba^2+ -> BaSO4 v} $$`;

  @ViewChild('mathContent') public element;

  result: string;

  private map = {};

  constructor() { }

  public getVariable(name: string): any {
    return this.variables[this.map[name]];
  }

  public setVariable(name: string, value: any): void {
    this.variables[this.map[name]] = value;
  }

  ngOnInit(): void {
    const words = [];
    let n = 0;
    let insideMath = false;
    this.variables = ['$x'];
    const openings = [];
    this.math.split(' ').forEach((word) => {
      console.log(word);
      if (word.includes('$$')) {
        insideMath = !insideMath;
      }
      if (word.includes('{')) {
        n++;
        openings.push(word);
      }
      if (word.includes('}')) { n--; openings.pop(); }
      if (this.variables.includes(word)) {
        word = word.replace('$', '\\$');
        if (insideMath) { word = `\\text{ ${word} }`; }
        word = ` ${word} ${openings.join('')}`;
        console.log(word);
      }
      words.push(word);
    });
    this.math = words.join(' ');
    console.log(this.math);

    // this.setVariable('x', 4);
    // this.setVariable('y', 1);
    // this.setVariable('z', 2);
  }

  ngAfterContentInit(): void {
    MathJax.Hub.Queue(
      [
        'Typeset',
        MathJax.Hub,
        () => {

          let html = this.element.nativeElement.innerHTML;
          console.log(html);
          const name = '$x';
          const regexp = new RegExp(name.replace('$', '\\$'), 'g');
          html = html.replace(regexp, '<input>');
          this.element.nativeElement.innerHTML = html;
        }
      ]);
  }

  submit(): void {
    const x = this.getVariable('x');
    const y = this.getVariable('y');
    const z = this.getVariable('z');

    if (x === 4 && y === 1 && z === 2) {
      this.result = 'Poprawna odpowiedź!';
    }
  }


}
