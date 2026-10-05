export interface Round {
  id: number;
  circleId: number;
  roundNumber: number;
  startDate: string;
  endDate: string;
  recipientId: number;
  status: 'upcoming' | 'active' | 'completed';
}