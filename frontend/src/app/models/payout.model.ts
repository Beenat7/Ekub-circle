export interface Payout {
  id: number;
  circleId: number;
  roundId: number;
  memberId: number;
  amount: number;
  paidAt: string;
  status: string;
}

export interface CreatePayoutRequest {
  circleId: number;
  roundId: number;
  memberId: number;
  amount: number;
  recordedBy: number;
}