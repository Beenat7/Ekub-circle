import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { Circle } from '../../models/circle';
import { CircleMember } from '../../models/circle-member';
import { Round } from '../../models/round.model';
import { CircleService } from '../../services/circle';
import { MemberService } from '../../services/member';
import { RoundService } from '../../services/round';

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
  private readonly roundService = inject(RoundService);

  readonly circles = signal<Circle[]>([]);
  readonly rounds = signal<Round[]>([]);
  readonly loadingRounds = signal(true);
  readonly roundsError = signal<string | null>(null);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly joiningCircleId = signal<string | number | null>(null);
  readonly joinedCircleIds = signal<Array<string | number>>([]);
  readonly availableOrderNumbers = signal<number[]>([]);
  readonly loadingOrderNumbers = signal(false);
  readonly submittingJoin = signal(false);
  readonly joinError = signal<string | null>(null);
  readonly joinSuccessMsg = signal<string | null>(null);
  readonly lockingCircleId = signal<string | number | null>(null);
  readonly lockError = signal<string | null>(null);
  readonly lockSuccessMsg = signal<string | null>(null);
  readonly roundActionCircleId = signal<string | number | null>(null);
  readonly roundActionError = signal<string | null>(null);
  readonly roundActionSuccess = signal<string | null>(null);

  selectedOrderNumber = 0;

  constructor() {
    this.loadCircles();
    this.loadRounds();
  }

  isJoined(circleId: string | number): boolean {
    return this.joinedCircleIds().some((id) => String(id) === String(circleId));
  }

  myCircles(): Circle[] {
    return this.circles().filter(
      (circle) => this.isOrganizer(circle) || this.isJoined(circle.id)
    );
  }

  availableCircles(): Circle[] {
    return this.circles().filter(
      (circle) => circle.status === 'NotStarted' && !this.isJoined(circle.id)
    );
  }

  circleSections(): Array<{ title: string; description: string; circles: Circle[] }> {
    return [
      {
        title: 'Your circles',
        description: 'Circles you created or have joined.',
        circles: this.myCircles()
      },
      {
        title: 'Available to join',
        description: 'Open circles accepting new members.',
        circles: this.availableCircles()
      }
    ];
  }

  hasCirclesToShow(): boolean {
    return this.myCircles().length > 0 || this.availableCircles().length > 0;
  }

  isOrganizer(circle: Circle): boolean {
    return Number(circle.organizerId) === Number(localStorage.getItem('memberId'));
  }

  canJoin(circle: Circle): boolean {
    return circle.status === 'NotStarted' && !this.isJoined(circle.id);
  }

  canStartRound(circle: Circle): boolean {
    if (!this.isOrganizer(circle) || circle.status !== 'Active' ||
        this.loadingRounds() || this.roundsError()) {
      return false;
    }

    const latestRound = this.rounds()
      .filter((round) => String(round.circleId) === String(circle.id))
      .sort((left, right) => right.roundNumber - left.roundNumber)[0];

    if (!latestRound) {
      return true;
    }
    if (latestRound.status !== 'completed' || !latestRound.completedAt) {
      return false;
    }

    const nextRoundTime = new Date(latestRound.completedAt).getTime() +
      circle.contributionIntervalDays * 24 * 60 * 60 * 1000;
    return Date.now() >= nextRoundTime;
  }

  loadCircles(): void {
    this.loading.set(true);
    this.error.set(null);

    const memberId = Number(localStorage.getItem('memberId'));
    if (!Number.isInteger(memberId) || memberId < 1) {
      this.error.set('Please log in again to view and join circles.');
      this.loading.set(false);
      return;
    }

    forkJoin({
      circles: this.circleService.getAll(),
      myCircles: this.circleService.getByMemberId(memberId)
    }).subscribe({
      next: ({ circles, myCircles }) => {
        this.circles.set(circles);
        this.joinedCircleIds.set(myCircles.map((circle) => circle.id));
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Failed to load circles:', err);
        this.error.set('Failed to load circles.');
        this.loading.set(false);
      }
    });
  }

  private loadRounds(): void {
    this.roundService.getRounds().subscribe({
      next: (rounds) => {
        this.rounds.set(rounds);
        this.loadingRounds.set(false);
      },
      error: (err) => {
        console.error('Failed to load circle round status:', err);
        this.roundsError.set('Could not load round status. Try refreshing before starting a round.');
        this.loadingRounds.set(false);
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

  lockCircle(circle: Circle): void {
    const memberId = Number(localStorage.getItem('memberId'));
    if (!Number.isInteger(memberId) || memberId < 1) {
      this.lockError.set('Please log in again before locking a circle.');
      return;
    }

    this.lockingCircleId.set(circle.id);
    this.lockError.set(null);
    this.lockSuccessMsg.set(null);

    this.circleService.lock(circle.id, memberId).subscribe({
      next: (lockedCircle) => {
        this.circles.update((circles) =>
          circles.map((item) =>
            String(item.id) === String(lockedCircle.id) ? lockedCircle : item
          )
        );
        this.lockSuccessMsg.set(`${lockedCircle.name} is locked. No more members can join.`);
        this.lockingCircleId.set(null);
      },
      error: (err) => {
        console.error('Failed to lock circle:', err);
        const msg = err?.error?.message || (typeof err?.error === 'string' ? err.error : 'Failed to lock circle.');
        this.lockError.set(msg);
        this.lockingCircleId.set(null);
      }
    });
  }

  startRound(circle: Circle): void {
    if (!this.canStartRound(circle) || this.roundActionCircleId() === circle.id) {
      return;
    }

    this.roundActionCircleId.set(circle.id);
    this.roundActionError.set(null);
    this.roundActionSuccess.set(null);
    const memberId = Number(localStorage.getItem('memberId'));
    this.circleService.openNextRound(circle.id, memberId).subscribe({
      next: (round) => {
        this.rounds.update((rounds) => [...rounds, round]);
        this.roundActionSuccess.set(
          `Round ${round.roundNumber} is now open for ${circle.name}.`
        );
        this.roundActionCircleId.set(null);
      },
      error: (err) => {
        console.error('Failed to start circle round:', err);
        this.roundActionError.set(
          err?.error?.message ?? 'Could not start the round. Check the circle status and interval.'
        );
        this.roundActionCircleId.set(null);
      }
    });
  }
}