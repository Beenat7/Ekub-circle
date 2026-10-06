import { Component, OnInit, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { forkJoin } from 'rxjs';
import { Payout as PayoutModel } from '../models/payout.model';
import { Payout as PayoutService } from '../services/payout';
import { Round } from '../models/round.model';
import { RoundService } from '../services/round';

@Component({
  selector: 'app-payout',
  imports: [DatePipe],
  templateUrl: './payout.html',
  styleUrl: './payout.scss'
})
export class Payout implements OnInit {
  private readonly payoutService = inject(PayoutService);
  private readonly roundService = inject(RoundService);
  payouts: PayoutModel[] = [];
  rounds: Round[] = [];
  loading = true;
  busyRoundId: number | null = null;
  error: string | null = null;
  success: string | null = null;
  private readonly memberId = Number(localStorage.getItem('memberId'));

  ngOnInit(): void {
    if (!Number.isInteger(this.memberId) || this.memberId < 1) {
      this.error = 'Please log in again to view or record payouts.';
      this.loading = false;
      return;
    }

    this.loadData();
  }

  get eligibleRounds(): Round[] {
    return this.rounds.filter(
      (round) =>
        round.status === 'open' &&
        round.organizerId === this.memberId &&
        round.memberCount > 0 &&
        round.paymentsReceived === round.memberCount
    );
  }

  recordPayout(round: Round): void {
    this.error = null;
    this.success = null;
    this.busyRoundId = round.id;

    this.payoutService.create({
      circleId: round.circleId,
      roundId: round.id,
      memberId: round.recipientId,
      amount: round.expectedPayoutAmount,
      recordedBy: this.memberId
    }).subscribe({
      next: (payout) => {
        this.payouts = [...this.payouts, payout];
        this.rounds = this.rounds.filter((item) => item.id !== round.id);
        this.success = `Payout recorded for Circle #${round.circleId}, Round ${round.roundNumber}.`;
        this.busyRoundId = null;
        this.loadData();
      },
      error: (error) => {
        console.error('Failed to record payout:', error);
        this.error = error?.error?.message ?? 'Could not record payout. Please try again.';
        this.busyRoundId = null;
      }
    });
  }

  private loadData(): void {
    this.loading = true;
    forkJoin({
      rounds: this.roundService.getRounds(),
      payouts: this.payoutService.getPayouts()
    }).subscribe({
      next: ({ rounds, payouts }) => {
        this.rounds = rounds;
        this.payouts = payouts;
        this.loading = false;
      },
      error: (error) => {
        console.error('Failed to load payout data:', error);
        this.error = 'Could not load rounds or payout history.';
        this.loading = false;
      }
    });
  }
}