import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MultipleChoiceComponent } from './multiple-choice.component';
import { BrowseComponent } from './browse/browse.component';
import { RouterModule } from '@angular/router';
import { PreviewComponent } from './preview/preview.component';
import { TestPreviewComponent } from './test-preview/test-preview.component';
import { TestShowComponent } from './test-show/test-show.component';
import { QuestionShowComponent } from './question-show/question-show.component';
import { AnswerShowComponent } from './answer-show/answer-show.component';
import { FormsModule } from '@angular/forms';
import { QuestionAnsweredComponent } from './question-answered/question-answered.component';

@NgModule({
  declarations: [MultipleChoiceComponent, BrowseComponent, PreviewComponent, TestPreviewComponent, TestShowComponent, QuestionShowComponent, AnswerShowComponent, QuestionAnsweredComponent],
  imports: [
    CommonModule,
    RouterModule,
    FormsModule
  ]
})
export class MultipleChoiceModule { }
