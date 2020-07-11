import { NgModule } from '@angular/core';
import { Routes } from '@angular/router';
import { CreateComponent } from './create/create.component';
import { EditComponent } from './edit/edit.component';
import { BrowseComponent } from './browse/browse.component';
import { ShowComponent } from './show/show.component';
import { DeleteComponent } from './delete/delete.component';

export const problemRoutes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'browse' },
  { path: 'browse', component: BrowseComponent },
  { path: 'create', component: CreateComponent },
  { path: ':id/edit', component: EditComponent },
  { path: ':id/delete', component: DeleteComponent },
  { path: ':id', component: ShowComponent },
];
