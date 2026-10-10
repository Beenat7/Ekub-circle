import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { CircleService } from './circle';

describe('CircleService', () => {
  let service: CircleService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(CircleService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('loads circles for a member through the circles API', () => {
    const http = TestBed.inject(HttpTestingController);
    service.getByMemberId(7).subscribe();
    const request = http.expectOne('http://localhost:5500/api/v1/circles/member/7');
    expect(request.request.method).toBe('GET');
    request.flush([]);
    http.verify();
  });
});
