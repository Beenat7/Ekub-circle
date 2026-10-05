import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { User } from '../models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly baseUrl = 'http://localhost:5000/api/v1/auth';
  
  public readonly currentUser = signal<User | null>(this.getStoredUser());

  constructor(private http: HttpClient) {}

  signup(data: {
    username: string;
    firstName: string;
    middleName: string;
    lastName: string;
    phonenumber: string;
    password: string;
  }): Observable<User> {
    return this.http.post<User>(`${this.baseUrl}/signup`, data).pipe(
      tap((user) => this.setUserSession(user))
    );
  }

  login(credentials: { username: string; password: string }): Observable<User> {
    return this.http.post<User>(`${this.baseUrl}/login`, credentials).pipe(
      tap((user) => this.setUserSession(user))
    );
  }

  logout(): void {
    localStorage.removeItem('ekub_user');
    this.currentUser.set(null);
  }

  getUserId(): number | null {
    const user = this.currentUser();
    return user ? user.id : null;
  }

  isLoggedIn(): boolean {
    return this.currentUser() !== null;
  }

  private setUserSession(user: User): void {
    if (user && user.id) {
      localStorage.setItem('ekub_user', JSON.stringify(user));
      this.currentUser.set(user);
    }
  }

  private getStoredUser(): User | null {
    try {
      const stored = localStorage.getItem('ekub_user');
      return stored ? JSON.parse(stored) : null;
    } catch {
      return null;
    }
  }
}
