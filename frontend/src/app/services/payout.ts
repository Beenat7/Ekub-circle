import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../config/api.config';
import { CreatePayoutRequest, Payout as PayoutModel } from '../models/payout.model';

@Injectable({
  providedIn: 'root'
})
export class Payout {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${API_CONFIG.baseUrl}/payouts`;

  getPayouts(): Observable<PayoutModel[]> {
    return this.http.get<PayoutModel[]>(this.apiUrl);
  }

  getPayoutsByMember(memberId: number): Observable<PayoutModel[]> {
    return this.http.get<PayoutModel[]>(`${this.apiUrl}/member/${memberId}`);
  }

  create(request: CreatePayoutRequest): Observable<PayoutModel> {
    return this.http.post<PayoutModel>(this.apiUrl, request);
  }
}
