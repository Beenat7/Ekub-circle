import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api.config';

export interface SignupPayload {
  firstName: string;
  middleName: string;
  lastName: string;
  phoneNumber: string;
  username: string;
  password: string;
}

export interface LoginResponse {
  id: number;
  username: string;
  firstName: string;
  middleName: string;
  lastName: string;
  phonenumber: string;
  createdAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class Auth {

  private readonly apiUrl = `${API_CONFIG.baseUrl}/auth`;

  constructor(private http: HttpClient) {}

  signup(data: SignupPayload): Observable<any> {
    return this.http.post(`${this.apiUrl}/signup`, data);
  }

  register(data: SignupPayload): Observable<any> {
    return this.signup(data);
  }

  login(data: { phoneNumber: string; password: string }): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, data);
  }
}