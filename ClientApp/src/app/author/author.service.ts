import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Author } from './author';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AuthorService {
  url = `${environment.url}/api/v1`;

  constructor(private http: HttpClient) { }

  getAuthor(id: any): Observable<Author> {
    return this.http.get<Author>(`${this.url}/authors/${id}`);
  }

  getSelf() {
    return this.http.get<Author>(`${this.url}/authors/self`);
  }
}
