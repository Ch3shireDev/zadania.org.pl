import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CreateComponent } from './create/create.component';
import { EditComponent } from './edit/edit.component';
import { BrowseComponent } from './browse/browse.component';
import { ShowDetailsComponent } from './show-details/show-details.component';
import { DeleteComponent } from './delete/delete.component';

export const problemRoutes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'browse' },
  { path: 'browse', component: BrowseComponent },
  { path: 'create', component: CreateComponent },
  { path: ':id/edit', component: EditComponent },
  { path: ':id/delete', component: DeleteComponent },
  { path: ':id', component: ShowDetailsComponent },
];

// @NgModule({
//   imports: [RouterModule.forRoot(problemRoutes)],
//   exports: [RouterModule],
//   // providers: [
//   //   {
//   //     provide: HTTP_INTERCEPTORS,
//   //     useClass: InterceptorService,
//   //     multi: true,
//   //   },
//   // ],
// })
// export class ProblemRoutingModule {}
