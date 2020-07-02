import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthorComponent } from './author.component';
import { BrowserModule } from '@angular/platform-browser';
import { ProblemModule } from '../problem/problem.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [AuthorComponent],
  imports: [CommonModule,
    RouterModule,
    BrowserModule, ProblemModule],
  exports: [AuthorComponent],
})
export class AuthorModule { }
