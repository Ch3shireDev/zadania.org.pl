import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { MultipleChoiceTest } from './multiple-choice-test';
import { MultipleChoiceQuestion } from './multiple-choice-question';
import { MultipleChoiceAnswer } from './multiple-choice-answer';

@Injectable({
  providedIn: 'root'
})
export class MultipleChoiceService {

  host = environment.url;
  url = `${this.host}/api/v1/multiple-choice`;

  constructor(private http: HttpClient) { }

  public getTests() {
    return this.http.get(this.url);
  }

  public getTest(link: string): Observable<MultipleChoiceTest> {
    return this.http.get<MultipleChoiceTest>(`${this.host}${link}`);
  }

  public getTestById(id: string): Observable<MultipleChoiceTest> {
    return this.http.get<MultipleChoiceTest>(`${this.url}/${id}`);
  }

  public getQuestion(link: string): Observable<MultipleChoiceQuestion> {
    return this.http.get<MultipleChoiceQuestion>(`${this.host}${link}`);
  }

  public getAnswer(link: string): Observable<MultipleChoiceAnswer> {
    return this.http.get<MultipleChoiceAnswer>(`${this.host}${link}`);
  }
}
