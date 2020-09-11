import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ExerciseComponent } from './exercise/exercise.component';
import { exerciseRoutes } from './exercise/exercise-routing.module';

const routes: Routes = [
  { path: '', component: ExerciseComponent, children: exerciseRoutes }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
