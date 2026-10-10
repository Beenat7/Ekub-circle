import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { Circles } from './circles';
import { CircleService } from '../../services/circle';
import { MemberService } from '../../services/member';

describe('Circles', () => {
  let component: Circles;
  let fixture: ComponentFixture<Circles>;

  beforeEach(async () => {
    localStorage.setItem('memberId', '1');
    await TestBed.configureTestingModule({
      imports: [Circles],
      providers: [
        provideRouter([]),
        { provide: CircleService, useValue: { getAll: () => of([]), getByMemberId: () => of([]) } },
        { provide: MemberService, useValue: { getByCircleId: () => of([]) } }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Circles);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('separates circles already joined from open circles available to join', () => {
    const ownedCircle = {
      id: 1,
      name: 'My circle',
      contributionAmount: 100,
      maxMembers: 3,
      contributionIntervalDays: 7,
      organizerId: 1,
      status: 'NotStarted' as const
    };
    const availableCircle = { ...ownedCircle, id: 2, organizerId: 8 };
    const joinedCircle = { ...ownedCircle, id: 3, organizerId: 8, status: 'Active' as const };
    component.circles.set([ownedCircle, availableCircle, joinedCircle]);
    component.joinedCircleIds.set([ownedCircle.id, joinedCircle.id]);

    expect(component.myCircles().map((circle) => circle.id)).toEqual([1, 3]);
    expect(component.availableCircles().map((circle) => circle.id)).toEqual([2]);
  });
});
