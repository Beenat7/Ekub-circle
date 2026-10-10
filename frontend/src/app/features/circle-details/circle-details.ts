import { DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { Circle } from '../../models/circle';
import { CircleService } from '../../services/circle';

@Component({
  selector: 'app-circle-details',
  imports: [
    DatePipe,
    RouterLink,
    MatButtonModule,
    MatCardModule,
    MatChipsModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './circle-details.html',
  styleUrl: './circle-details.scss'
})
export class CircleDetails {
  private readonly route = inject(ActivatedRoute);
  private readonly circleService = inject(CircleService);

  readonly circle = signal<Circle | null>(null);
  readonly loading = signal(true);
  readonly starting = signal(false);
  readonly error = signal<string | null>(null);
  readonly successMsg = signal<string | null>(null);

  readonly memberId = Number(localStorage.getItem('memberId'));

  constructor() {
    const circleId = this.route.snapshot.paramMap.get('id');

    if (!circleId) {
      this.error.set('Circle ID was not provided.');
      this.loading.set(false);
      return;
    }

    this.loadCircle(circleId);
  }

  get isOrganizer(): boolean {
    const c = this.circle();
    return !!c && Number(c.organizerId) === this.memberId;
  }

  get isForming(): boolean {
    const c = this.circle();
    return !!c && (c.status === 'Forming' || c.status === 'NotStarted');
  }

  startCircle(): void {
    const c = this.circle();
    if (!c) return;

    if (!confirm(`Are you sure you want to start circle "${c.name}"? This will lock membership and generate all rotation rounds.`)) {
      return;
    }

    this.starting.set(true);
    this.error.set(null);
    this.successMsg.set(null);

    this.circleService.lock(c.id, this.memberId).subscribe({
      next: (updatedCircle) => {
        this.circle.set(updatedCircle);
        this.starting.set(false);
        this.successMsg.set('Circle started successfully! Rounds and recipient rotation sequence have been generated.');
      },
      error: (err) => {
        console.error('Failed to start circle:', err);
        this.starting.set(false);
        const msg = err?.error?.message || 'Could not start circle. Ensure at least two members have joined and you are the organizer.';
        this.error.set(msg);
      }
    });
  }

  private loadCircle(id: string): void {
    this.circleService.getById(id).subscribe({
      next: (circle) => {
        this.circle.set(circle);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load circle details.');
        this.loading.set(false);
      }
    });
  }
}