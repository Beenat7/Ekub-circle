import { Component } from '@angular/core';
import { Payout as PayoutModel } from '../models/payout.model';
import { Payout as PayoutService } from '../services/payout';

@Component({
  selector: 'app-payout',
  imports: [],
  templateUrl: './payout.html',
  styleUrl: './payout.scss'
})
export class Payout {

  payouts: PayoutModel[] = [];

  constructor(private payoutService: PayoutService) {}

  ngOnInit(): void {
    this.payouts = this.payoutService.getPayouts();
  }
}