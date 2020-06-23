import { NgModule } from '@angular/core'
import { CommonModule, DatePipe } from '@angular/common'
import { ProblemComponent } from './problem.component'
import { CreateComponent } from './create/create.component'
import { EditComponent } from './edit/edit.component'
import { EditorComponent } from './editor/editor.component'
import { DeleteComponent } from './delete/delete.component'
import { ShowComponent } from './show/show.component'
// import { ProblemRoutingModule } from './problem-routing.module';
import { BrowseComponent } from './browse/browse.component'
import { FormsModule } from '@angular/forms'
import { ShowDetailsComponent } from './show-details/show-details.component'
import { RouterModule } from '@angular/router'
import { AnswerModule } from '../answer/answer.module'
import { PointsComponent } from './points/points.component'
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome'
// import { SafePipe } from './safe.pipe';
import { QuillModule } from 'ngx-quill'

@NgModule({
  declarations: [
    ProblemComponent,
    CreateComponent,
    EditComponent,
    EditorComponent,
    DeleteComponent,
    ShowComponent,
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
  ],
  providers: [DatePipe],
  exports: [ProblemComponent],
})
export class ProblemModule {}
