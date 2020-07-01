import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { ProblemService } from '../problem.service';
import { ActivatedRoute, Route, Router } from '@angular/router';

@Component({
  selector: 'app-browse',
  templateUrl: './browse.component.html',
  styleUrls: ['./browse.component.css'],
})
export class BrowseComponent implements OnInit, AfterViewInit {
  problems: any[];
  searchQuery: string;
  lastSearchQuery: string;

  pageNum: number;
  totalPages: number;
  tags: string;

  @ViewChild('searchInput') private searchInputElement: ElementRef;

  constructor(private problemService: ProblemService, private router: Router, private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe(params => {
      this.getAllProblems(params);
    });
  }

  ngAfterViewInit(): void {
    this.searchInputElement.nativeElement.focus();
  }

  getAllProblems(params=null) {
    // console.log('xxx');
    // console.log(params);
    this.problemService.getProblems(params).subscribe((res) => {
      this.pageNum = res.pageNum;
      this.totalPages = res.totalPages;
      this.problems = res.problems;
    });
  }

  search(): void {
    if (this.searchQuery === null || this.searchQuery === undefined) {
      this.getAllProblems();
      return;
    }
    if (this.searchQuery === this.lastSearchQuery) { return; }
    const searchQuery = this.searchQuery.trim();
    this.problemService.searchProblems(searchQuery).subscribe((problems) => {
      this.problems = problems;
      this.lastSearchQuery = searchQuery;
    });
  }

  previousPage() {
    this.setPage(this.pageNum - 1);
  }

  nextPage() {
    this.setPage(this.pageNum + 1);
  }

  setPage(page) {
    const tags = this.activatedRoute.snapshot.queryParams.tags;
    const queryParams = { tags, page };
    // console.log(queryParams);
    this.router.navigate(['/problems/browse'], { queryParams });
  }
}
