import { Component, inject, signal, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';

import { MemberService } from '../../services/member';
import { CircleMember, AddCircleMemberRequest } from '../../models/circle-member';

@Component({
  selector: 'app-members',
  imports: [
    DatePipe,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatTableModule
  ],
  templateUrl: './members.html',
  styleUrl: './members.scss'
})
export class Members implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly memberService = inject(MemberService);

  readonly members = signal<CircleMember[]>([]);
  readonly loading = signal(false);
  readonly submitting = signal(false);
  readonly error = signal<string | null>(null);
  readonly successMsg = signal<string | null>(null);

  readonly memberForm = this.fb.nonNullable.group({
    circleId: [0, [Validators.required, Validators.min(1)]],
    memberId: [0, [Validators.required, Validators.min(1)]],
    orderNumber: [0, [Validators.required, Validators.min(1)]]
  });

  displayedColumns: string[] = ['id', 'circleId', 'memberId', 'memberName', 'orderNumber', 'joinedAt'];

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers(): void {
    const circleId = this.memberForm.controls.circleId.value;
    if (!circleId || circleId < 1) {
      this.members.set([]);
      this.loading.set(false);
      return;
    }

    this.loading.set(true);
    this.error.set(null);
    this.memberService.getByCircleId(circleId).subscribe({
      next: (list) => {
        this.members.set(list || []);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Failed to load circle members:', err);
        this.members.set([]);
        this.error.set(err?.error?.message ?? 'Could not load members for this circle.');
        this.loading.set(false);
      }
    });
  }

  onSubmit(): void {
    if (this.memberForm.invalid) {
      this.memberForm.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.error.set(null);
    this.successMsg.set(null);

    const raw = this.memberForm.getRawValue();
    const payload: AddCircleMemberRequest = {
      circleId: Number(raw.circleId),
      memberId: Number(raw.memberId),
      orderNumber: Number(raw.orderNumber)
    };

    this.memberService.addMember(payload).subscribe({
      next: (newMember) => {
        this.submitting.set(false);
        this.successMsg.set(`Member #${payload.memberId} added successfully to Circle #${payload.circleId}!`);
        this.members.update(list => [...list, newMember]);
      },
      error: (err) => {
        console.error('Failed to add circle member:', err);
        const msg = err?.error?.message || (typeof err?.error === 'string' ? err.error : 'Failed to add member to circle.');
        this.error.set(msg);
        this.submitting.set(false);
      }
    });
  }
}
