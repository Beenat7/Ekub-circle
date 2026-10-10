import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../config/api.config';
import { CreatePaymentRequest, Payment } from '../models/payment.model';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${API_CONFIG.baseUrl}/payments`;

  getPayments(): Observable<Payment[]> {
    return this.http.get<Payment[]>(this.apiUrl);
  }

  getPaymentsByMember(memberId: number): Observable<Payment[]> {
    return this.http.get<Payment[]>(`${this.apiUrl}/member/${memberId}`);
  }

  getPendingByOrganizer(organizerId: number): Observable<Payment[]> {
    return this.http.get<Payment[]>(`${this.apiUrl}/pending/organizer/${organizerId}`);
  }

  create(request: CreatePaymentRequest): Observable<Payment> {
    return this.http.post<Payment>(this.apiUrl, request);
  }

  review(paymentId: number, reviewerId: number, approve: boolean): Observable<Payment> {
    return this.http.patch<Payment>(`${this.apiUrl}/${paymentId}/review`, {
      reviewerId,
      approve
    });
  }
}