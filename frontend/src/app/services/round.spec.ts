import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { RoundService } from './round';

describe('RoundService', () => {
  let service: RoundService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(RoundService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
