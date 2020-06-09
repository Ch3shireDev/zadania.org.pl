import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthorComponent } from './author.component';
import { BrowserModule } from '@angular/platform-browser';

@NgModule({
  declarations: [AuthorComponent],
  imports: [CommonModule, BrowserModule],
  exports: [AuthorComponent],
})
export class AuthorModule {}
