import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Dashboard } from './dashboard';
import { CircleService } from '../services/circle';
import { PaymentService } from '../services/payment';
import { RoundService } from '../services/round';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';

describe('Dashboard', () => {
  let component: Dashboard;
  let fixture: ComponentFixture<Dashboard>;

  beforeEach(async () => {
    localStorage.setItem('memberId', '1');
    localStorage.setItem('memberName', 'Test Member');
    await TestBed.configureTestingModule({
      imports: [Dashboard],
      providers: [
        provideRouter([]),
        { provide: CircleService, useValue: { getByMemberId: () => of([]), openNextRound: () => of({}) } },
        { provide: RoundService, useValue: { getRounds: () => of([]) } },
        { provide: PaymentService, useValue: { getPaymentsByMember: () => of([]) } }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Dashboard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('stops loading and prompts login when no member session is stored', async () => {
    localStorage.removeItem('memberId');
    const loggedOutFixture = TestBed.createComponent(Dashboard);
    const loggedOutComponent = loggedOutFixture.componentInstance;
    loggedOutFixture.detectChanges();
    await loggedOutFixture.whenStable();

    expect(loggedOutComponent.loginRequired).toBe(true);
    expect(loggedOutComponent.loading).toBe(false);
    expect(loggedOutComponent.loadingRounds).toBe(false);
    expect(loggedOutComponent.loadingPayments).toBe(false);
  });

  it('shows zero totals when backend responses contain no records', () => {
    expect(component.totalCircles).toBe(0);
    expect(component.activeRounds).toBe(0);
    expect(component.totalContributed).toBe(0);
    expect(component.nextPayment).toBe(0);
  });

  it('allows only the circle creator to start an active circle round', () => {
    const creatorCircle = {
      id: 1,
      name: 'Creator circle',
      contributionAmount: 100,
      maxMembers: 3,
      contributionIntervalDays: 5,
      organizerId: 1,
      status: 'Active' as const
    };
    const joinedCircle = { ...creatorCircle, id: 2, organizerId: 9 };

    expect(component.canStartRound(creatorCircle)).toBe(true);
    expect(component.canStartRound(joinedCircle)).toBe(false);
  });

  it('waits for the configured interval before opening another round', () => {
    const circle = {
      id: 1,
      name: 'Creator circle',
      contributionAmount: 100,
      maxMembers: 3,
      contributionIntervalDays: 5,
      organizerId: 1,
      status: 'Active' as const
    };

    component.rounds = [{
      id: 1,
      circleId: 1,
      organizerId: 1,
      roundNumber: 1,
      startDate: new Date().toISOString(),
      endDate: new Date().toISOString(),
      recipientId: 1,
      status: 'completed',
      memberCount: 3,
      paymentsReceived: 3,
      contributionAmount: 100,
      expectedPayoutAmount: 300,
      completedAt: new Date().toISOString()
    }];
    expect(component.canStartRound(circle)).toBe(false);

    component.rounds[0].completedAt = new Date(Date.now() - 6 * 24 * 60 * 60 * 1000).toISOString();
    expect(component.canStartRound(circle)).toBe(true);
  });
});
