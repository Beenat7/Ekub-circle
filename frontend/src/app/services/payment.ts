import { Injectable } from '@angular/core';
import { Payment } from '../models/payment.model';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {

  private payments: Payment[] = [
    {
      id: 1,
      circleId: 1,
      roundId: 1,
      memberId: 101,
      amount: 5000,
      paymentDate: '2026-10-05',
      bankName: 'Commercial Bank of Ethiopia',
      receiptUrl: 'receipts/payment-1.jpg',
      status: 'approved'
    },
    {
      id: 2,
      circleId: 1,
      roundId: 1,
      memberId: 102,
      amount: 5000,
      paymentDate: '2026-10-06',
      bankName: 'Awash Bank',
      receiptUrl: 'receipts/payment-2.jpg',
      status: 'approved'
    },
    {
      id: 3,
      circleId: 1,
      roundId: 2,
      memberId: 101,
      amount: 5000,
      paymentDate: '2026-11-05',
      bankName: 'Commercial Bank of Ethiopia',
      receiptUrl: 'receipts/payment-3.jpg',
      status: 'pending'
    }
  ];

  getPayments(): Payment[] {
    return this.payments;
  }

  getPaymentById(id: number): Payment | undefined {
    return this.payments.find(payment => payment.id === id);
  }

  getPaymentsByRound(roundId: number): Payment[] {
    return this.payments.filter(payment => payment.roundId === roundId);
  }

  getPaymentsByMember(memberId: number): Payment[] {
    return this.payments.filter(payment => payment.memberId === memberId);
  }
}