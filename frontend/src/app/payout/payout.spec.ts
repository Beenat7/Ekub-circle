import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Payout } from './payout';

describe('Payout', () => {
  let component: Payout;
  let fixture: ComponentFixture<Payout>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Payout],
    }).compileComponents();

    fixture = TestBed.createComponent(Payout);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
