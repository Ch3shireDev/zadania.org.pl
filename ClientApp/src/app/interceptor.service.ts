import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
} from '@angular/common/http';
import { AuthService } from './auth.service';
import { Observable } from 'rxjs';
import { mergeMap, catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class InterceptorService implements HttpInterceptor {
  constructor(
    private auth: AuthService
    ) { }

  profile;

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    this.auth.userProfile$.subscribe((profile) => {
      this.profile = profile;
    });

    return this.auth.getTokenSilently$().pipe(
      mergeMap((token) => {
        const tokenReq = req.clone({
          setHeaders: {
            Authorization: `Bearer ${token}`,
            Profile: `${JSON.stringify(this.profile)}`,
          },
        });
        return next.handle(tokenReq);
      }),
      catchError((err) => {
        const tokenReq = req.clone({
          setHeaders: {},
        });
        return next.handle(tokenReq);
      })
    );
  }
}
