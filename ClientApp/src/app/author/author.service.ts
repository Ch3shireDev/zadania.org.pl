import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Author } from './author';

@Injectable({
  providedIn: 'root',
})
export class AuthorService {
  url = 'http://localhost:5000/api/v1';

  constructor(private http: HttpClient) {}

  getAuthor(id: any): Observable<Author> {
    return this.http.get<Author>(`${this.url}/authors/${id}`);
  }

  getSelf() {
    return this.http.get<Author>(`${this.url}/authors/self`);
  }
}
