import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { InterceptorService } from './interceptor.service';
import { HomeComponent } from './home/home.component';
import { problemRoutes } from './problem/problem-routing.module';
import { AuthorComponent } from './author/author.component';

const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'problems' },
  { path: 'home', component: HomeComponent },
  { path: 'problems', children: problemRoutes },
  { path: 'authors/self', component: AuthorComponent },
  { path: 'authors/:id', component: AuthorComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],

  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: InterceptorService,
      multi: true,
    },
  ],
})
export class AppRoutingModule { }
