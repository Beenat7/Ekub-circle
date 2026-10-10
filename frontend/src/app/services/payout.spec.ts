import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { Payout } from './payout';

describe('Payout', () => {
  let service: Payout;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(Payout);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
