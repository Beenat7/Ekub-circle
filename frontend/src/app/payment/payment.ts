import { Component, OnInit, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { Circle } from '../models/circle';
import { Payment as PaymentModel } from '../models/payment.model';
import { PaymentService } from '../services/payment';
import { Round } from '../models/round.model';
import { RoundService } from '../services/round';
import { CircleService } from '../services/circle';

@Component({
  selector: 'app-payment',
  imports: [DatePipe, FormsModule],
  templateUrl: './payment.html',
  styleUrl: './payment.scss'
})
export class Payment implements OnInit {
  private readonly paymentService = inject(PaymentService);
  private readonly roundService = inject(RoundService);
  private readonly circleService = inject(CircleService);
  payments: PaymentModel[] = [];
  pendingPayments: PaymentModel[] = [];
  circles: Circle[] = [];
  rounds: Round[] = [];
  paymentMethod: 'Cash' | 'BankTransfer' | 'MobileMoney' | 'Other' = 'BankTransfer';
  loading = true;
  error: string | null = null;
  success: string | null = null;
  busyRoundId: number | null = null;
  selectedRoundId: number | null = null;
  paymentAmount = 0;
  bankName = '';
  transactionId = '';
  busyPaymentId: number | null = null;
  busyCircleId: number | null = null;
  private readonly memberId = Number(localStorage.getItem('memberId'));

  ngOnInit(): void {
    if (!Number.isInteger(this.memberId) || this.memberId < 1) {
      this.error = 'Please log in again to view or record contributions.';
      this.loading = false;
      return;
    }

    this.loadData();
  }

  hasPaid(roundId: number): boolean {
    return this.payments.some(
      (payment) => payment.roundId === roundId && payment.status !== 'rejected'
    );
  }

  get openRounds(): Round[] {
    return this.rounds.filter(
      (round) => round.status === 'open' && !this.hasPaid(round.id)
    );
  }

  get organizerCircles(): Circle[] {
    return this.circles.filter(
      (circle) => Number(circle.organizerId) === this.memberId && circle.status === 'Active'
    );
  }

  latestRound(circleId: string | number): Round | undefined {
    return this.rounds
      .filter((round) => String(round.circleId) === String(circleId))
      .sort((a, b) => b.roundNumber - a.roundNumber)[0];
  }

  nextRoundEligibleAt(circle: Circle): Date | null {
    const lastRound = this.latestRound(circle.id);
    if (!lastRound) {
      return null;
    }
    if (lastRound.status !== 'completed' || !lastRound.completedAt) {
      return new Date(Number.NaN);
    }
    return new Date(
      new Date(lastRound.completedAt).getTime() +
      circle.contributionIntervalDays * 24 * 60 * 60 * 1000
    );
  }

  canOpenNextRound(circle: Circle): boolean {
    const lastRound = this.latestRound(circle.id);
    if (!lastRound) {
      return true;
    }
    const eligibleAt = this.nextRoundEligibleAt(circle);
    return Boolean(eligibleAt && !Number.isNaN(eligibleAt.getTime()) && eligibleAt <= new Date());
  }

  isOpeningCircle(circle: Circle): boolean {
    return this.busyCircleId === Number(circle.id);
  }

  openPaymentForm(round: Round): void {
    this.selectedRoundId = round.id;
    this.paymentAmount = round.contributionAmount;
    this.bankName = '';
    this.transactionId = '';
    this.error = null;
  }

  recordPayment(round: Round): void {
    this.error = null;
    this.success = null;
    if (!this.bankName.trim() || !this.transactionId.trim()) {
      this.error = 'Enter the bank or provider name and transaction ID.';
      return;
    }
    if (Number(this.paymentAmount) !== round.contributionAmount) {
      this.error = `The contribution must be exactly ${round.contributionAmount} ETB.`;
      return;
    }

    this.busyRoundId = round.id;

    this.paymentService.create({
      circleId: round.circleId,
      roundId: round.id,
      memberId: this.memberId,
      amount: Number(this.paymentAmount),
      paymentMethod: this.paymentMethod,
      transactionId: this.transactionId.trim(),
      bankName: this.bankName.trim(),
      recordedBy: this.memberId
    }).subscribe({
      next: (payment) => {
        this.payments = [...this.payments, payment];
        this.success = `Payment submitted for Circle #${round.circleId}, Round ${round.roundNumber}. It is waiting for organizer approval.`;
        this.busyRoundId = null;
        this.selectedRoundId = null;
        this.loadData();
      },
      error: (error) => {
        console.error('Failed to record contribution:', error);
        this.error = error?.error?.message ?? 'Could not record contribution. Please try again.';
        this.busyRoundId = null;
      }
    });
  }

  reviewPayment(payment: PaymentModel, approve: boolean): void {
    this.busyPaymentId = payment.id;
    this.error = null;
    this.success = null;
    this.paymentService.review(payment.id, this.memberId, approve).subscribe({
      next: () => {
        this.pendingPayments = this.pendingPayments.filter((item) => item.id !== payment.id);
        this.success = approve ? 'Payment approved.' : 'Payment rejected. The member can submit corrected details.';
        this.busyPaymentId = null;
        this.loadData();
      },
      error: (error) => {
        console.error('Failed to review payment:', error);
        this.error = error?.error?.message ?? 'Could not review this payment.';
        this.busyPaymentId = null;
      }
    });
  }

  openNextRound(circle: Circle): void {
    this.busyCircleId = Number(circle.id);
    this.error = null;
    this.success = null;
    this.circleService.openNextRound(circle.id, this.memberId).subscribe({
      next: (round) => {
        this.rounds = [...this.rounds, round];
        this.success = `Round ${round.roundNumber} is open for Circle #${circle.id}.`;
        this.busyCircleId = null;
      },
      error: (error) => {
        console.error('Failed to open next round:', error);
        this.error = error?.error?.message ?? 'Could not open the next round.';
        this.busyCircleId = null;
      }
    });
  }

  private loadData(): void {
    this.loading = true;
    forkJoin({
      rounds: this.roundService.getRounds(),
      payments: this.paymentService.getPaymentsByMember(this.memberId),
      pendingPayments: this.paymentService.getPendingByOrganizer(this.memberId),
      circles: this.circleService.getAll()
    }).subscribe({
      next: ({ rounds, payments, pendingPayments, circles }) => {
        this.rounds = rounds;
        this.payments = payments;
        this.pendingPayments = pendingPayments;
        this.circles = circles;
        this.loading = false;
      },
      error: (error) => {
        console.error('Failed to load contribution data:', error);
        this.error = 'Could not load rounds or your contribution history.';
        this.loading = false;
      }
    });
  }
}