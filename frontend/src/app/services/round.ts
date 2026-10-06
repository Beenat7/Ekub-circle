import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../config/api.config';
import { Round } from '../models/round.model';

@Injectable({
  providedIn: 'root'
})
export class RoundService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${API_CONFIG.baseUrl}/rounds`;

  getRounds(): Observable<Round[]> {
    return this.http.get<Round[]>(this.apiUrl);
  }

  getRoundById(id: number): Observable<Round> {
    return this.http.get<Round>(`${this.apiUrl}/${id}`);
  }

  getRoundsByCircle(circleId: number): Observable<Round[]> {
    return this.http.get<Round[]>(`${this.apiUrl}/circle/${circleId}`);
  }
}