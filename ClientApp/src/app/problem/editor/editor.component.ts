import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core'
import { Problem } from '../problem'
import { Location } from '@angular/common'

@Component({
  selector: 'app-editor',
  templateUrl: './editor.component.html',
  styleUrls: ['./editor.component.css'],
})
export class EditorComponent implements OnInit {
  @Input() problem = new Problem()
  @Input() title: string
  // @Output() submit: EventEmitter<Problem> = new EventEmitter<Problem>();
  editorStyle = { height: '300px' }

  editorConfig = {
    toolbar: [
      ['bold', 'italic', 'underline', 'strike'], // toggled buttons
      ['blockquote', 'code-block'],
      [{ header: 1 }, { header: 2 }], // custom button values
      [{ list: 'ordered' }, { list: 'bullet' }],
      [{ script: 'sub' }, { script: 'super' }], // superscript/subscript
      [{ indent: '-1' }, { indent: '+1' }], // outdent/indent
      [{ direction: 'rtl' }], // text direction
      [{ size: ['small', false, 'large', 'huge'] }], // custom dropdown
      [{ header: [1, 2, 3, 4, 5, 6, false] }],
      [{ color: [] }, { background: [] }], // dropdown with defaults from theme
      [{ font: [] }],
      [{ align: [] }],
      ['clean'], // remove formatting button
      ['link'], // link
    ],
  }
  constructor() {}

  ngOnInit(): void {}

  // onSubmit() {
  //   this.submit.emit(this.problem);
  // }

  // goBack() {
  //   this.location.back();
  // }
}
