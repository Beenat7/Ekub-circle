import { Component } from '@angular/core';
import { Payment as PaymentModel } from '../models/payment.model';
import { PaymentService } from '../services/payment';

@Component({
  selector: 'app-payment',
  imports: [],
  templateUrl: './payment.html',
  styleUrl: './payment.scss'
})
export class Payment {

  payments: PaymentModel[] = [];

  constructor(private paymentService: PaymentService) {}

  ngOnInit(): void {
    this.payments = this.paymentService.getPayments();
  }
}