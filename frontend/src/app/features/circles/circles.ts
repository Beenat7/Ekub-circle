import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { Circle } from '../../models/circle';
import { CircleService } from '../../services/circle';

@Component({
  selector: 'app-circles',
  imports: [
    RouterLink,
    MatButtonModule,
    MatCardModule,
    MatChipsModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './circles.html',
  styleUrl: './circles.scss'
})
export class Circles {
  private readonly circleService = inject(CircleService);

  readonly circles = signal<Circle[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  constructor() {
    this.loadCircles();
  }

  private loadCircles(): void {
    this.loading.set(true);
    this.error.set(null);

    this.circleService.getAll().subscribe({
      next: (circles) => {
        this.circles.set(circles);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load circles.');
        this.loading.set(false);
      }
    });
  }
}