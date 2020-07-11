import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { ProblemComponent } from './problem.component';
import { CreateComponent } from './create/create.component';
import { EditComponent } from './edit/edit.component';
import { EditorComponent } from './editor/editor.component';
import { DeleteComponent } from './delete/delete.component';
import { PreviewComponent as ProblemPreviewComponent } from './preview/preview.component';
import { BrowseComponent } from './browse/browse.component';
import { FormsModule } from '@angular/forms';
import { ShowDetailsComponent } from './show-details/show-details.component';
import { RouterModule } from '@angular/router';
import { AnswerModule } from './answer/answer.module';
import { PointsComponent } from './points/points.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { MarkdownModule } from 'ngx-markdown';
import { QuillModule } from 'ngx-quill';

@NgModule({
  declarations: [
    ProblemComponent,
    CreateComponent,
    EditComponent,
    EditorComponent,
    DeleteComponent,
    ProblemPreviewComponent,
    BrowseComponent,
    ShowDetailsComponent,
    PointsComponent,
    // SafePipe
  ],
  imports: [
    CommonModule,
    RouterModule,
    AnswerModule,
    FormsModule,
    FontAwesomeModule,
    QuillModule.forRoot(),
    // MarkdownModule.forRoot()
  ],
  providers: [DatePipe],
  exports: [ProblemComponent, ProblemPreviewComponent],
})
export class ProblemModule { }
