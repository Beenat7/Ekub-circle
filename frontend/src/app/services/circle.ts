import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../config/api.config';
import { Circle, CreateCircleRequest } from '../models/circle';

@Injectable({
  providedIn: 'root'
})
export class CircleService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${API_CONFIG.baseUrl}/circles`;

  getAll(): Observable<Circle[]> {
    return this.http.get<Circle[]>(this.apiUrl);
  }

  getByMemberId(memberId: string | number): Observable<Circle[]> {
    return this.http.get<Circle[]>(
      `${API_CONFIG.baseUrl}/members/${memberId}/circles`
    );
  }

  getById(id: string | number): Observable<Circle> {
    return this.http.get<Circle>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateCircleRequest): Observable<Circle> {
    return this.http.post<Circle>(this.apiUrl, request);
  }
}