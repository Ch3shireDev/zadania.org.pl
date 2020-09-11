import { ExerciseModule } from './exercise/exercise.module';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [AppComponent],
  imports: [FormsModule,
    BrowserModule,
    AppRoutingModule,
    ExerciseModule],
  bootstrap: [AppComponent]
})
export class AppModule { }
