import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowExperimentalComponent } from './show-experimental.component';

describe('ShowExperimentalComponent', () => {
  let component: ShowExperimentalComponent;
  let fixture: ComponentFixture<ShowExperimentalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ShowExperimentalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShowExperimentalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
