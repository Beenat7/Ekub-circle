import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../config/api.config';
import { CircleMember } from '../models/circle-member';

@Injectable({
  providedIn: 'root'
})
export class MemberService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl = `${API_CONFIG.baseUrl}/CircleMembers`;

  getByCircleId(circleId: string): Observable<CircleMember[]> {
    return this.http.get<CircleMember[]>(`${this.apiUrl}/circle/${circleId}`);
  }
}