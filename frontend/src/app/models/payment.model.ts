export interface Payment {
  id: number;
  circleId: number;
  roundId: number;
  memberId: number;
  amount: number;
  paymentDate: string;
  bankName: string;
  receiptUrl: string;
  status: 'pending' | 'approved' | 'rejected';
}