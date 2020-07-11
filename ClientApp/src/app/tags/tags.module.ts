import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListComponent } from './list/list.component';
import { TagsComponent } from './tags.component';
import { RouterModule } from '@angular/router';



@NgModule({
  declarations: [ListComponent, TagsComponent],
  imports: [
    CommonModule,
    RouterModule,
  ],
  exports: [TagsComponent]
})
export class TagsModule { }
