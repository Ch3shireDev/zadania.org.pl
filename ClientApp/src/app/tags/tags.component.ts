import { Component, OnInit } from '@angular/core';
import { TagService } from './tag.service';
import { Tag } from '../problem/tag';

@Component({
  selector: 'app-tags',
  templateUrl: './tags.component.html',
  styleUrls: ['./tags.component.css']
})
export class TagsComponent implements OnInit {

  public tags: Tag[];

  constructor(private tagService: TagService) { }

  ngOnInit(): void {
    this.tagService.getTags().subscribe(tags => { this.tags = tags; });
  }

}
