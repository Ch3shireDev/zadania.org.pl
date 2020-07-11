import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Answer } from './answer';
import { environment } from 'src/environments/environment';
@Injectable({
  providedIn: 'root',
})
export class AnswerService {

  host = environment.url;
  url = `${environment.url}/api/v1`;

  constructor(private http: HttpClient) { }

  getAnswerFromLink(link: string): Observable<Answer> {
    return this.http.get<Answer>(`${this.host}${link}`);
  }

  getAnswers(problemId: number): Observable<Answer[]> {
    return this.http.get<Answer[]>(`${this.url}/problems/${problemId}/answers`);
  }

  getAnswer(problemId: number, answerId: number) {
    return this.http.get<Answer>(
      `${this.url}/problems/${problemId}/answers/${answerId}`
    );
  }

  postAnswer(problemId: number, answer: {}) {
    return this.http.post(`${this.url}/problems/${problemId}/answers`, answer);
  }

  putAnswer(problemId: number, answerId: number, answer: {}) {
    return this.http.put(
      `${this.url}/problems/${problemId}/answers/${answerId}`,
      answer
    );
  }

  deleteAnswer(problemId: number, answerId: number) {
    return this.http.delete(
      `${this.url}/problems/${problemId}/answers/${answerId}`
    );
  }

  upvoteAnswer(answer: Answer) {
    const problemId = answer.problemId;
    const answerId = answer.id;
    return this.http.post(
      `${this.url}/problems/${problemId}/answers/${answerId}/upvote`,
      {}
    );
  }

  downvoteAnswer(answer: Answer) {
    const problemId = answer.problemId;
    const answerId = answer.id;
    return this.http.post(
      `${this.url}/problems/${problemId}/answers/${answerId}/downvote`,
      {}
    );
  }

  approveAnswer(answer: Answer): Observable<any> {
    const answerId = answer.id;
    const problemId = answer.problemId;
    return this.http.post(`${this.url}/problems/${problemId}/answers/${answerId}/approve`, {});
  }

  disapproveAnswer(answer: Answer): Observable<any> {
    const answerId = answer.id;
    const problemId = answer.problemId;
    return this.http.post(`${this.url}/problems/${problemId}/answers/${answerId}/disapprove`, {});
  }
}
