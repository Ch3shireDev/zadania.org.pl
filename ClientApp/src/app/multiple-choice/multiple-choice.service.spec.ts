import { TestBed } from '@angular/core/testing';

import { MultipleChoiceService } from './multiple-choice.service';

describe('MultipleChoiceService', () => {
  let service: MultipleChoiceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MultipleChoiceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
