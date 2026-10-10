export interface Round {
  id: number;
  circleId: number;
  organizerId: number;
  roundNumber: number;
  startDate: string;
  endDate: string;
  recipientId: number;
  status: 'pending' | 'open' | 'completed';
  memberCount: number;
  paymentsReceived: number;
  contributionAmount: number;
  expectedPayoutAmount: number;
  completedAt?: string | null;
}