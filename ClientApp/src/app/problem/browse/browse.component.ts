import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { ProblemService } from '../problem.service';

@Component({
  selector: 'app-browse',
  templateUrl: './browse.component.html',
  styleUrls: ['./browse.component.css'],
})
export class BrowseComponent implements OnInit, AfterViewInit {
  problems: any[];
  searchQuery: string;
  lastSearchQuery: string;
  @ViewChild('searchInput') private searchInputElement: ElementRef;

  constructor(private problemService: ProblemService) { }

  ngOnInit(): void {
    this.getAllProblems();
  }

  ngAfterViewInit(): void {
    this.searchInputElement.nativeElement.focus();
  }

  getAllProblems() {
    this.problemService.getProblems().subscribe((problems) => {
      this.problems = problems;
      console.log(this.problems);
    });
  }

  search(): void {
    if (this.searchQuery === null || this.searchQuery === undefined) {
      this.getAllProblems();
      return;
    }
    if (this.searchQuery === this.lastSearchQuery) { return; }
    if (this.searchQuery.trim() === this.lastSearchQuery.trim()) { return; }
    const searchQuery = this.searchQuery.trim();
    this.problemService.searchProblems(searchQuery).subscribe((problems) => {
      this.problems = problems;
      this.lastSearchQuery = searchQuery;
    });
  }
}
