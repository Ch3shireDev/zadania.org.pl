// import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { BrowseComponent } from './browse/browse.component';
import { TestShowComponent } from './test-show/test-show.component';

export const multipleChoiceRoutes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'browse' },
  { path: 'browse', component: BrowseComponent },
  { path: ':id', component: TestShowComponent },
];
