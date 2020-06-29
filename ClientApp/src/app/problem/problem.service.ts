import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Problem } from './problem';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ProblemService {
  url = `${environment.url}/api/v1`;

  constructor(private http: HttpClient) { }


  downvoteProblem(id: number) {
    return this.http.put(`${this.url}/problems/${id}/downvote`, {});
  }
  upvoteProblem(id: number) {
    return this.http.put(`${this.url}/problems/${id}/upvote`, {});
  }

  getProblems(): Observable<Problem[]> {
    return this.http.get<Problem[]>(`${this.url}/problems`);
  }

  getProblem(id): Observable<Problem> {
    return this.http.get<Problem>(`${this.url}/problems/${id}`);
  }

  postProblem(problem: Problem) {
    console.log(this.url);
    return this.http.post(`${this.url}/problems`, problem);
  }

  putProblem(id: number, problem: Problem) {
    return this.http.put(`${this.url}/problems/${id}`, problem);
  }

  deleteProblem(id: number) {
    return this.http.delete(`${this.url}/problems/${id}`);
  }

  searchProblems(query: string): Observable<Problem[]> {
    if (query === null || query === undefined) { return null; }
    return this.http.get<Problem[]>(`${this.url}/problems/search?query=${query}`);
  }
}
