export interface Payment {
  id: number;
  circleId: number;
  roundId: number;
  memberId: number;
  amount: number;
  paymentMethod: string;
  transactionId: string | null;
  paidAt: string;
  status: string;
}

export interface CreatePaymentRequest {
  circleId: number;
  roundId: number;
  memberId: number;
  amount: number;
  paymentMethod: 'Cash' | 'BankTransfer' | 'MobileMoney' | 'Other';
  transactionId: string | null;
  recordedBy: number;
}