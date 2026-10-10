import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { CircleService } from '../../services/circle';
import { CreateCircleRequest } from '../../models/circle';

@Component({
  selector: 'app-circle-form',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './circle-form.html',
  styleUrl: './circle-form.scss'
})
export class CircleForm {
  private readonly formBuilder = inject(FormBuilder);
  private readonly circleService = inject(CircleService);
  private readonly router = inject(Router);

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly circleForm = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
    contributionAmount: [0, [Validators.required, Validators.min(1)]],
    maxMembers: [0, [Validators.required, Validators.min(2)]],
    contributionIntervalDays: [0, [Validators.required, Validators.min(1)]]
  });

  onSubmit(): void {
    if (this.circleForm.invalid) {
      this.circleForm.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    const raw = this.circleForm.getRawValue();
    const organizerId = Number(localStorage.getItem('memberId'));
    if (!Number.isInteger(organizerId) || organizerId < 1) {
      this.error.set('Please log in again before creating a circle.');
      this.loading.set(false);
      return;
    }

    const request: CreateCircleRequest = {
      name: raw.name.trim(),
      contributionAmount: Number(raw.contributionAmount),
      maxMembers: Number(raw.maxMembers),
      contributionIntervalDays: Number(raw.contributionIntervalDays),
      organizerId
    };

    this.circleService.create(request).subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/circles']);
      },
      error: (err) => {
        console.error('Failed to create circle:', err);
        const msg = err?.error?.message || (typeof err?.error === 'string' ? err.error : 'Failed to create circle.');
        this.error.set(msg);
        this.loading.set(false);
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/circles']);
  }
}