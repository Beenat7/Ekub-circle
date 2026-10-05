import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Circle, CreateCirclePayload, JoinCircleResponse } from '../models';

@Injectable({
  providedIn: 'root'
})
export class CircleService {
  private readonly baseUrl = 'http://localhost:5000/api/v1/circles';

  constructor(private http: HttpClient) {}

  getAllCircles(): Observable<Circle[]> {
    return this.http.get<Circle[]>(this.baseUrl);
  }

  getAvailableCircles(): Observable<Circle[]> {
    return this.http.get<Circle[]>(`${this.baseUrl}/available`);
  }

  getMyCircles(memberId: number): Observable<Circle[]> {
    return this.http.get<Circle[]>(`${this.baseUrl}/my-circles/${memberId}`);
  }

  createCircle(payload: CreateCirclePayload): Observable<Circle> {
    return this.http.post<Circle>(this.baseUrl, payload);
  }

  joinCircle(circleId: number, memberId: number): Observable<JoinCircleResponse> {
    return this.http.post<JoinCircleResponse>(`${this.baseUrl}/${circleId}/join`, { memberId });
  }
}
