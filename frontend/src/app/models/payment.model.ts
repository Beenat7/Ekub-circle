export interface Payment {
  id: number;
  circleId: number;
  roundId: number;
  memberId: number;
  amount: number;
  paymentMethod: string;
  transactionId: string | null;
  paidAt: string;
  status: 'pending' | 'approved' | 'rejected';
  bankName: string;
  circleName: string | null;
  memberName: string | null;
  reviewedByMemberId: number | null;
  reviewedAt: string | null;
}

export interface CreatePaymentRequest {
  circleId: number;
  roundId: number;
  memberId: number;
  amount: number;
  paymentMethod: 'Cash' | 'BankTransfer' | 'MobileMoney' | 'Other';
  transactionId: string;
  bankName: string;
  recordedBy: number;
}