import { TestBed } from '@angular/core/testing';

import { Farmers } from './farmers.service';

describe('Farmers', () => {
  let service: Farmers;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Farmers);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
