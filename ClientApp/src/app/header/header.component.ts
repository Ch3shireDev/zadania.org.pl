import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth.service';
import { Route, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

  isNewest = false;
  isHighest = false;

  constructor(public auth: AuthService, public route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const newest = params.newest === 'true';
      const highest = params.highest === 'true';
      this.isHighest = highest;
      this.isNewest = newest;
    });
  }

}
