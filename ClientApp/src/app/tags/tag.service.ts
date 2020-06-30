import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { Tag } from '../problem/tag';

@Injectable({
  providedIn: 'root'
})
export class TagService {

  url = `${environment.url}/api/v1`;

  constructor(private http: HttpClient) { }

  public getTags(): Observable<Tag[]> {
    return this.http.get<Tag[]>(`${this.url}/tags`);
  }
}
