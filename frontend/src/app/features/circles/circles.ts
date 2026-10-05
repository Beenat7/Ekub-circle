import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { Circle } from '../../models/circle';
import { CircleMember } from '../../models/circle-member';
import { CircleService } from '../../services/circle';
import { MemberService } from '../../services/member';

@Component({
  selector: 'app-circles',
  imports: [
    RouterLink,
    FormsModule,
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
  private readonly memberService = inject(MemberService);

  readonly circles = signal<Circle[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly joiningCircleId = signal<string | number | null>(null);
  readonly joinedCircleIds = signal<Array<string | number>>([]);
  readonly availableOrderNumbers = signal<number[]>([]);
  readonly loadingOrderNumbers = signal(false);
  readonly submittingJoin = signal(false);
  readonly joinError = signal<string | null>(null);
  readonly joinSuccessMsg = signal<string | null>(null);

  selectedOrderNumber = 0;

  constructor() {
    this.loadCircles();
  }

  isJoined(circleId: string | number): boolean {
    return this.joinedCircleIds().some((id) => String(id) === String(circleId));
  }

  loadCircles(): void {
    this.loading.set(true);
    this.error.set(null);

    this.circleService.getAll().subscribe({
      next: (circles) => {
        this.circles.set(circles || []);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Failed to load circles:', err);
        this.error.set('Failed to load circles.');
        this.loading.set(false);
      }
    });
  }

  openJoinForm(circle: Circle): void {
    if (this.joiningCircleId() === circle.id) {
      this.joiningCircleId.set(null);
      this.availableOrderNumbers.set([]);
      this.joinError.set(null);
      return;
    }

    const memberId = Number(localStorage.getItem('memberId'));
    if (!Number.isInteger(memberId) || memberId < 1) {
      this.joinError.set('Please log in again before joining a circle.');
      return;
    }

    this.joiningCircleId.set(circle.id);
    this.loadingOrderNumbers.set(true);
    this.availableOrderNumbers.set([]);
    this.joinError.set(null);
    this.joinSuccessMsg.set(null);

    this.memberService.getByCircleId(circle.id).subscribe({
      next: (members) => {
        if (this.joiningCircleId() !== circle.id) {
          return;
        }

        const existingMembers = members ?? [];
        const alreadyJoined = existingMembers.some(
          (member) => Number(member.memberId) === memberId
        );

        if (alreadyJoined) {
          this.joinedCircleIds.update((ids) =>
            ids.some((id) => String(id) === String(circle.id))
              ? ids
              : [...ids, circle.id]
          );
          this.joinError.set('You have already joined this circle.');
          this.loadingOrderNumbers.set(false);
          return;
        }

        const occupiedOrderNumbers = new Set(
          existingMembers.map((member: CircleMember) => Number(member.orderNumber))
        );
        const openOrderNumbers = Array.from(
          { length: Math.max(0, circle.maxMembers) },
          (_, index) => index + 1
        ).filter((orderNumber) => !occupiedOrderNumbers.has(orderNumber));

        this.availableOrderNumbers.set(openOrderNumbers);
        this.selectedOrderNumber = openOrderNumbers[0] ?? 0;
        this.loadingOrderNumbers.set(false);

        if (openOrderNumbers.length === 0) {
          this.joinError.set('This circle has no available member positions.');
        }
      },
      error: (err) => {
        if (this.joiningCircleId() !== circle.id) {
          return;
        }

        console.error('Failed to load circle members:', err);
        this.joinError.set('Could not check available positions. Please try again.');
        this.loadingOrderNumbers.set(false);
      }
    });
  }

  confirmJoin(circleId: string | number): void {
    const memberId = Number(localStorage.getItem('memberId'));
    const orderNumber = Number(this.selectedOrderNumber);
    if (!Number.isInteger(memberId) || memberId < 1) {
      this.joinError.set('Please log in again before joining a circle.');
      return;
    }
    if (!this.availableOrderNumbers().includes(orderNumber)) {
      this.joinError.set('Choose an available member position.');
      return;
    }

    this.submittingJoin.set(true);
    this.joinError.set(null);

    this.memberService.addMember({
      circleId: Number(circleId),
      memberId,
      orderNumber
    }).subscribe({
      next: () => {
        this.joinSuccessMsg.set('You have successfully joined the circle.');
        this.joinedCircleIds.update((ids) => [...ids, circleId]);
        this.joiningCircleId.set(null);
        this.submittingJoin.set(false);
      },
      error: (err) => {
        console.error('Failed to join circle:', err);
        const msg = err?.error?.message || (typeof err?.error === 'string' ? err.error : 'Failed to join circle.');
        this.joinError.set(msg);
        this.submittingJoin.set(false);
      }
    });
  }
}