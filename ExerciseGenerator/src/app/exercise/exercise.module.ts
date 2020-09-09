import { FormsModule } from '@angular/forms';
import { ExerciseComponent } from './exercise.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ShowComponent } from './show/show.component';
import { EditorComponent } from './editor/editor.component';
import { CreateComponent } from './create/create.component';
import { EditComponent } from './edit/edit.component';
import { DeleteComponent } from './delete/delete.component';
import { ListComponent } from './list/list.component';
import { RouterModule } from '@angular/router';
import { Exercise } from './exercise';



@NgModule({
  declarations: [
    ExerciseComponent,
    ShowComponent, EditorComponent, CreateComponent, EditComponent, DeleteComponent, ListComponent],
  imports: [
    CommonModule,
    FormsModule,
    RouterModule
  ],
  exports: [
    ExerciseComponent
  ]
})
export class ExerciseModule { }
