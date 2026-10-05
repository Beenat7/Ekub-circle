import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../config/api.config';
import { CircleMember, AddCircleMemberRequest } from '../models/circle-member';

@Injectable({
  providedIn: 'root'
})
export class MemberService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl = `${API_CONFIG.baseUrl}/circle-members`;

  getByCircleId(circleId: string | number): Observable<CircleMember[]> {
    return this.http.get<CircleMember[]>(`${this.apiUrl}/circle/${circleId}`);
  }

  addMember(data: AddCircleMemberRequest): Observable<CircleMember> {
    return this.http.post<CircleMember>(this.apiUrl, data);
  }
}