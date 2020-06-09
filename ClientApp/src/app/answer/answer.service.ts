import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Answer } from './answer';

@Injectable({
  providedIn: 'root',
})
export class AnswerService {
  url = 'http://localhost:5000/api/v1';

  constructor(private http: HttpClient) {}

  getAnswers(parentId: number): Observable<Answer[]> {
    return this.http.get<Answer[]>(`${this.url}/problems/${parentId}/answers`);
  }

  getAnswer(parentId: number, answerId: number) {
    return this.http.get<Answer>(
      `${this.url}/problems/${parentId}/answers/${answerId}`
    );
  }

  postAnswer(parentId: number, answer: {}) {
    return this.http.post(`${this.url}/problems/${parentId}/answers`, answer);
  }

  putAnswer(parentId: number, answerId: number, answer: {}) {
    return this.http.put(
      `${this.url}/problems/${parentId}/answers/${answerId}`,
      answer
    );
  }

  deleteAnswer(parentId: number, answerId: number) {
    return this.http.delete(
      `${this.url}/problems/${parentId}/answers/${answerId}`
    );
  }

  upvoteAnswer(parentId: number, answerId: number) {
    return this.http.put(
      `${this.url}/problems/${parentId}/answers/${answerId}/upvote`,
      {}
    );
  }

  downvoteAnswer(parentId: number, answerId: number) {
    return this.http.put(
      `${this.url}/problems/${parentId}/answers/${answerId}/downvote`,
      {}
    );
  }
}
