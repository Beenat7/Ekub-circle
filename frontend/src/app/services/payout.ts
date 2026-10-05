import { Injectable } from '@angular/core';
import { Payout as PayoutModel } from '../models/payout.model';

@Injectable({
  providedIn: 'root'
})
export class Payout {

  private payouts: PayoutModel[] = [
    {
      id: 1,
      circleId: 1,
      roundId: 1,
      memberId: 101,
      amount: 50000,
      payoutDate: '2026-10-10',
      bankName: 'Commercial Bank of Ethiopia',
      accountNumber: '100012345678',
      status: 'completed'
    },
    {
      id: 2,
      circleId: 1,
      roundId: 2,
      memberId: 102,
      amount: 50000,
      payoutDate: '2026-11-10',
      bankName: 'Awash Bank',
      accountNumber: '200012345678',
      status: 'pending'
    },
    {
      id: 3,
      circleId: 2,
      roundId: 1,
      memberId: 103,
      amount: 30000,
      payoutDate: '2026-10-15',
      bankName: 'Dashen Bank',
      accountNumber: '300012345678',
      status: 'pending'
    }
  ];

  getPayouts(): PayoutModel[] {
    return this.payouts;
  }

  getPayoutById(id: number): PayoutModel | undefined {
    return this.payouts.find(payout => payout.id === id);
  }

  getPayoutsByRound(roundId: number): PayoutModel[] {
    return this.payouts.filter(payout => payout.roundId === roundId);
  }

  getPayoutsByMember(memberId: number): PayoutModel[] {
    return this.payouts.filter(payout => payout.memberId === memberId);
  }
}