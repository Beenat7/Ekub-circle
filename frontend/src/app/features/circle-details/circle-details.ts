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
  readonly error = signal<string | null>(null);

  constructor() {
    const circleId = this.route.snapshot.paramMap.get('id');

    if (!circleId) {
      this.error.set('Circle ID was not provided.');
      this.loading.set(false);
      return;
    }

    this.loadCircle(circleId);
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