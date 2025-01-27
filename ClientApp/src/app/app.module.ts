import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { RouterModule } from '@angular/router';
import { ProblemModule } from './problem/problem.module';
import { HomeComponent } from './home/home.component';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { AuthorModule } from './author/author.module';

import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { FooterComponent } from './footer/footer.component';
import { HeaderComponent } from './header/header.component';

import { QuillModule } from 'ngx-quill';
// import { TagsComponent } from './tags/tags.component';

import { TagsModule } from './tags/tags.module';

import { MultipleChoiceModule } from './multiple-choice/multiple-choice.module';

@NgModule({
  declarations: [AppComponent, HomeComponent, FooterComponent, HeaderComponent],
  imports: [
    AuthorModule,
    FormsModule,
    ProblemModule,
    MultipleChoiceModule,
    HttpClientModule,
    BrowserModule,
    AppRoutingModule,
    RouterModule,
    FontAwesomeModule,
    TagsModule,
    QuillModule.forRoot()
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule { }
