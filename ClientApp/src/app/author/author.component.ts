import { Component, OnInit } from '@angular/core';
import { AuthorService } from './author.service';
import { Author } from './author';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';

@Component({
  selector: 'app-author',
  templateUrl: './author.component.html',
  styleUrls: ['./author.component.css'],
})
export class AuthorComponent implements OnInit {
  public author: Author;

  constructor(
    private authorService: AuthorService,
    private activatedRoute: ActivatedRoute,
    private location: Location
  ) { }

  ngOnInit(): void {
    const id = this.activatedRoute.snapshot.params.id;

    if (id === undefined) {
      this.authorService.getSelf().subscribe((author) => {
        this.author = author;
      });
    } else {
      this.authorService.getAuthor(id).subscribe((author) => {
        this.author = author;
      });
    }
  }

  goBack() {
    this.location.back();
  }
}
