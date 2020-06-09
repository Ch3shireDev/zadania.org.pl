import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class TestService {
  constructor(private http: HttpClient) {}

  public public() {
    return this.http.get('http://localhost:5000/api/v1/test/public', {
      responseType: 'text',
    });
  }
  public private() {
    return this.http.get('http://localhost:5000/api/v1/test/private', {
      responseType: 'text',
    });
  }
}
