import { TestBed } from '@angular/core/testing';
import { Payout } from './payout';

describe('Payout', () => {
  let service: Payout;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Payout);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
