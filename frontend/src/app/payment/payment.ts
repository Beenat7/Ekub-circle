import { Component, OnInit, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { Payment as PaymentModel } from '../models/payment.model';
import { PaymentService } from '../services/payment';
import { Round } from '../models/round.model';
import { RoundService } from '../services/round';

@Component({
  selector: 'app-payment',
  imports: [DatePipe, FormsModule],
  templateUrl: './payment.html',
  styleUrl: './payment.scss'
})
export class Payment implements OnInit {
  private readonly paymentService = inject(PaymentService);
  private readonly roundService = inject(RoundService);
  payments: PaymentModel[] = [];
  rounds: Round[] = [];
  paymentMethod: 'Cash' | 'BankTransfer' | 'MobileMoney' | 'Other' = 'Cash';
  loading = true;
  error: string | null = null;
  success: string | null = null;
  busyRoundId: number | null = null;
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
    return this.payments.some((payment) => payment.roundId === roundId);
  }

  get openRounds(): Round[] {
    return this.rounds.filter(
      (round) => round.status === 'open' && !this.hasPaid(round.id)
    );
  }

  recordPayment(round: Round): void {
    this.error = null;
    this.success = null;
    this.busyRoundId = round.id;

    this.paymentService.create({
      circleId: round.circleId,
      roundId: round.id,
      memberId: this.memberId,
      amount: round.contributionAmount,
      paymentMethod: this.paymentMethod,
      transactionId: null,
      recordedBy: this.memberId
    }).subscribe({
      next: (payment) => {
        this.payments = [...this.payments, payment];
        this.success = `Contribution recorded for Circle #${round.circleId}, Round ${round.roundNumber}.`;
        this.busyRoundId = null;
        this.loadData();
      },
      error: (error) => {
        console.error('Failed to record contribution:', error);
        this.error = error?.error?.message ?? 'Could not record contribution. Please try again.';
        this.busyRoundId = null;
      }
    });
  }

  private loadData(): void {
    this.loading = true;
    forkJoin({
      rounds: this.roundService.getRounds(),
      payments: this.paymentService.getPaymentsByMember(this.memberId)
    }).subscribe({
      next: ({ rounds, payments }) => {
        this.rounds = rounds;
        this.payments = payments;
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