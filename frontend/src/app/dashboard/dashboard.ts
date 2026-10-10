import { Component, OnInit, inject } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Circle } from '../models/circle';
import { Payment } from '../models/payment.model';
import { Round } from '../models/round.model';
import { CircleService } from '../services/circle';
import { PaymentService } from '../services/payment';
import { RoundService } from '../services/round';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink, DecimalPipe],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit {
  private readonly circleService = inject(CircleService);
  private readonly roundService = inject(RoundService);
  private readonly paymentService = inject(PaymentService);
  private readonly memberId = Number(localStorage.getItem('memberId'));

  memberName = localStorage.getItem('memberName') || 'Member';
  circles: Circle[] = [];
  rounds: Round[] = [];
  private allRounds: Round[] = [];
  payments: Payment[] = [];
  loading = true;
  loadingRounds = true;
  loadingPayments = true;
  error: string | null = null;
  loginRequired = false;
  roundsError: string | null = null;
  paymentsError: string | null = null;
  success: string | null = null;
  startingCircleId: string | number | null = null;

  get totalCircles(): number {
    return this.circles.length;
  }

  get activeRounds(): number {
    const circleIds = new Set(this.circles.map((circle) => String(circle.id)));
    return this.rounds.filter(
      (round) => circleIds.has(String(round.circleId)) && round.status === 'open'
    ).length;
  }

  get totalContributed(): number {
    return this.payments
      .filter((payment) => payment.status === 'approved')
      .reduce((sum, payment) => sum + payment.amount, 0);
  }

  get nextPayment(): number {
    const myOpenRound = this.rounds
      .filter((round) => round.status === 'open')
      .filter((round) => !this.payments.some(
        (payment) => payment.roundId === round.id && payment.status !== 'rejected'
      ))
      .sort((left, right) => new Date(left.endDate).getTime() - new Date(right.endDate).getTime())[0];
    return myOpenRound?.contributionAmount ?? 0;
  }

  isCircleCreator(circle: Circle): boolean {
    return Number(circle.organizerId) === this.memberId;
  }

  latestRound(circleId: string | number): Round | undefined {
    return this.rounds
      .filter((round) => String(round.circleId) === String(circleId))
      .sort((left, right) => right.roundNumber - left.roundNumber)[0];
  }

  private refreshMemberRounds(): void {
    const circleIds = new Set(this.circles.map((circle) => String(circle.id)));
    this.rounds = this.allRounds.filter((round) => circleIds.has(String(round.circleId)));
  }

  canStartRound(circle: Circle): boolean {
    if (!this.isCircleCreator(circle) || circle.status !== 'Active') {
      return false;
    }

    const latest = this.latestRound(circle.id);
    if (!latest) {
      return true;
    }
    if (latest.status !== 'completed' || !latest.completedAt) {
      return false;
    }

    const nextRoundTime = new Date(latest.completedAt).getTime() +
      circle.contributionIntervalDays * 24 * 60 * 60 * 1000;
    return Date.now() >= nextRoundTime;
  }

  startRound(circle: Circle): void {
    if (!this.canStartRound(circle) || this.startingCircleId === circle.id) {
      return;
    }

    this.startingCircleId = circle.id;
    this.error = null;
    this.success = null;
    this.circleService.openNextRound(circle.id, this.memberId).subscribe({
      next: (round) => {
        this.rounds = [...this.rounds, round];
        this.success = `Round ${round.roundNumber} is now open for ${circle.name}.`;
        this.startingCircleId = null;
      },
      error: (error) => {
        console.error('Failed to start circle round:', error);
        this.error = error?.error?.message ?? 'Could not start the round. Check the circle status and interval.';
        this.startingCircleId = null;
      }
    });
  }

  ngOnInit(): void {
    if (!Number.isInteger(this.memberId) || this.memberId < 1) {
      this.error = 'Please log in again to load your dashboard.';
      this.loginRequired = true;
      this.loading = false;
      this.loadingRounds = false;
      this.loadingPayments = false;
      return;
    }

    this.circleService.getByMemberId(this.memberId).subscribe({
      next: (circles) => {
        this.circles = circles;
        this.refreshMemberRounds();
        this.loading = false;
      },
      error: (error) => {
        console.error('Failed to load member circles:', error);
        this.error = 'Could not load your circles from the server. Please try again.';
        this.loading = false;
      }
    });

    this.roundService.getRounds().subscribe({
      next: (rounds) => {
        this.allRounds = rounds;
        this.refreshMemberRounds();
        this.loadingRounds = false;
      },
      error: (error) => {
        console.error('Failed to load dashboard rounds:', error);
        this.roundsError = 'Round information could not be loaded.';
        this.loadingRounds = false;
      }
    });

    this.paymentService.getPaymentsByMember(this.memberId).subscribe({
      next: (payments) => {
        this.payments = payments;
        this.loadingPayments = false;
      },
      error: (error) => {
        console.error('Failed to load dashboard payment history:', error);
        this.paymentsError = 'Payment totals could not be loaded.';
        this.loadingPayments = false;
      }
    });
  }
}