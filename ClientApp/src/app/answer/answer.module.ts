import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AnswerComponent } from './answer.component';
import { CreateComponent } from './create/create.component';
import { BrowseComponent } from './browse/browse.component';
import { EditComponent } from './edit/edit.component';
import { EditorComponent } from './editor/editor.component';
import { DeleteComponent } from './delete/delete.component';
import { ShowComponent } from './show/show.component';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { PointsComponent } from './points/points.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { SafeHtmlPipe } from './safe-html.pipe';

@NgModule({
  declarations: [
    AnswerComponent,
    CreateComponent,
    BrowseComponent,
    EditComponent,
    EditorComponent,
    DeleteComponent,
    ShowComponent,
    PointsComponent,
    SafeHtmlPipe,
  ],
  imports: [CommonModule, FormsModule, RouterModule, FontAwesomeModule],
  exports: [AnswerComponent, SafeHtmlPipe],
})
export class AnswerModule {}
