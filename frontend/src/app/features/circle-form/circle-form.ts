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
    name: ['', Validators.required],
    contributionAmount: [0, [Validators.required, Validators.min(1)]],
    maxMembers: [2, [Validators.required, Validators.min(2)]],
    contributionIntervalDays: [7, [Validators.required, Validators.min(1)]]
  });

  onSubmit(): void {
    if (this.circleForm.invalid) {
      this.circleForm.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    const request = this.circleForm.getRawValue();

    this.circleService.create(request).subscribe({
      next: () => {
        this.router.navigate(['/circles']);
      },
      error: () => {
        this.error.set('Failed to create circle.');
        this.loading.set(false);
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/circles']);
  }
}