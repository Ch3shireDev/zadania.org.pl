import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AnswerComponent } from './answer.component';
import { AnswerCreateComponent } from './answer-create/answer-create.component';
import { AnswersBrowseComponent } from './answers-browse/answers-browse.component';
import { AnswerEditComponent } from './answer-edit/answer-edit.component';
import { AnswerEditorComponent } from './answer-editor/answer-editor.component';
import { AnswerDeleteComponent } from './answer-delete/answer-delete.component';
import { AnswerShowComponent } from './answer-show/answer-show.component';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AnswerPointsComponent } from './answer-points/answer-points.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { SafeHtmlPipe } from './safe-html.pipe';
import { QuillModule } from 'ngx-quill';

@NgModule({
  declarations: [
    AnswerComponent,
    AnswerCreateComponent,
    AnswersBrowseComponent,
    AnswerEditComponent,
    AnswerEditorComponent,
    AnswerDeleteComponent,
    AnswerShowComponent,
    AnswerPointsComponent,
    SafeHtmlPipe,
  ],
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    FontAwesomeModule,
    QuillModule.forRoot(),
  ],
  exports: [AnswerComponent, SafeHtmlPipe, AnswersBrowseComponent],
})
export class AnswerModule { }
