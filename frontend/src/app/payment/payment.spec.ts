import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Payment } from './payment';
import { PaymentService } from '../services/payment';
import { RoundService } from '../services/round';
import { CircleService } from '../services/circle';
import { of } from 'rxjs';

describe('Payment', () => {
  let component: Payment;
  let fixture: ComponentFixture<Payment>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Payment],
      providers: [
        { provide: PaymentService, useValue: { getPaymentsByMember: () => of([]), getPendingByOrganizer: () => of([]), create: () => of({}), review: () => of({}) } },
        { provide: RoundService, useValue: { getRounds: () => of([]) } },
        { provide: CircleService, useValue: { getAll: () => of([]), openNextRound: () => of({}) } }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Payment);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('allows opening the first round and gates later rounds by the contribution interval', () => {
    const circle = {
      id: 12,
      name: 'Savings circle',
      contributionAmount: 100,
      maxMembers: 3,
      contributionIntervalDays: 7,
      organizerId: 1,
      status: 'Active' as const
    };

    expect(component.canOpenNextRound(circle)).toBe(true);

    component.rounds = [{
      id: 20,
      circleId: 12,
      organizerId: 1,
      roundNumber: 1,
      startDate: '2026-01-01T00:00:00Z',
      endDate: '2026-01-08T00:00:00Z',
      recipientId: 1,
      status: 'completed',
      memberCount: 3,
      paymentsReceived: 3,
      contributionAmount: 100,
      expectedPayoutAmount: 300,
      completedAt: new Date(Date.now() - 8 * 24 * 60 * 60 * 1000).toISOString()
    }];

    expect(component.canOpenNextRound(circle)).toBe(true);

    component.rounds[0].completedAt = new Date().toISOString();
    expect(component.canOpenNextRound(circle)).toBe(false);
  });
});
