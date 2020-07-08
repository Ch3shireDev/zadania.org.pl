import { Component, OnInit, Input } from '@angular/core';
import { Problem } from '../problem';
import { editorConfig, editorStyle } from '../../editor-config';
import { Tag } from '../tag';

@Component({
  selector: 'app-editor',
  templateUrl: './editor.component.html',
  styleUrls: ['./editor.component.css'],
})
export class EditorComponent implements OnInit {
  @Input() problem = new Problem();
  @Input() title: string;

  tagText: string;
  // @Output() submit: EventEmitter<Problem> = new EventEmitter<Problem>();
  editorStyle = editorStyle;
  editorConfig = editorConfig;
  constructor() { }

  ngOnInit(): void { }

  addTag() {
    if (this.tagText === undefined || this.tagText.trim() === '') { return; }
    const tag = this.tagText.trim();
    if (!this.problem.tags.includes(tag)) {
      const tagElement = new Tag();
      tagElement.name = tag;
      this.problem.tags.push(tagElement);
    }
    this.tagText = '';
  }

  removeTag(tag: string) {
    const tags = this.problem.tags.map(element => element.name);
    if (!tags.includes(tag)) { return; }
    const index: number = tags.indexOf(tag);
    if (index !== -1) {
      this.problem.tags.splice(index, 1);
    }
  }
}
