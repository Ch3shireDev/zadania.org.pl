import { ExerciseService } from './../exercise.service';
import { Exercise } from './../exercise';
import { Component, OnInit, AfterContentInit, PipeTransform, Pipe, ViewChild, ɵConsole, OnChanges } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
@Pipe({
  name: 'split'
})
export class SplitPipe implements PipeTransform {
  transform(val: string): string[] {
    return val.split(' ');
  }
}

@Pipe({
  name: 'sanitizeHtml'
})
export class SanitizeHtmlPipe implements PipeTransform {

  constructor(private sanitizer: DomSanitizer) { }

  transform(v: string): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(v);
  }
}

@Pipe({ name: 'safeHtml' })
export class SafeHtmlPipe implements PipeTransform {
  constructor(private sanitizer: DomSanitizer) { }

  transform(style) {
    console.log(style);
    console.log(this.sanitizer.bypassSecurityTrustHtml(style));
    return this.sanitizer.bypassSecurityTrustHtml(style);
  }
}
@Component({
  selector: 'app-show',
  templateUrl: './show.component.html',
  styleUrls: ['./show.component.css']
})
export class ShowComponent implements OnInit, AfterContentInit, OnChanges {

  public id: string;
  public exercise: Exercise;
  public submitted: boolean;
  public content: string;
  public title: string;
  public answers: { name, description, value, userAnswer; }[];
  public result: string;

  @ViewChild('contentElement') public contentElement;
  @ViewChild('mathContent') public element;

  constructor(private route: ActivatedRoute, private exerciseService: ExerciseService) {
  }

  ngOnChanges(): void {
  }

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');
    this.exerciseService.getExercise(this.id).subscribe(exercise => {
      this.exercise = exercise;
      this.initialize();
    });
  }
  initialize(): void {
    [this.content, this.answers] = this.exercise.initialize();
    this.title = this.exercise.title;
    this.submitted = false;
    this.result = undefined;
  }

  submit(): void {
    this.submitted = true;
    let correct = true;
    const messages = [];
    this.answers.forEach(answer => {
      if (answer.userAnswer === undefined) {
        this.submitted = false;
        messages.push(`Brak wartości ${answer.name}.`);
      }
      if (this.submitted === false) { return; }
      if (answer.value != answer.userAnswer) {
        correct = false;
        messages.push(`Błąd wartości "${answer.name}". Podana wartość: ${answer.userAnswer}, poprawna wartość: ${answer.value}. `);
      }
    });
    if (!correct || !this.submitted) {
      this.result = messages.join('\n');
      return;
    }

    this.result = 'Poprawna odpowiedź!';
    const ex = this;
    setTimeout(() => { ex.next(); }, 1000);
  }

  next(): void {
    if (!this.submitted) { return; }
    this.initialize();
  }


  ngAfterContentInit(): void {
    MathJax.Hub.Queue(
      [
        'Typeset',
        MathJax.Hub
      ]);
  }

}
