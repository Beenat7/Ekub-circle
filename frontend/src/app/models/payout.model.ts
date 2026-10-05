export interface Payout {
  id: number;
  circleId: number;
  roundId: number;
  memberId: number;
  amount: number;
  payoutDate: string;
  bankName: string;
  accountNumber: string;
  status: 'pending' | 'completed' | 'failed';
}