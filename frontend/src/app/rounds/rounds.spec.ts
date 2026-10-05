import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Rounds } from './rounds';

describe('Rounds', () => {
  let component: Rounds;
  let fixture: ComponentFixture<Rounds>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Rounds],
    }).compileComponents();

    fixture = TestBed.createComponent(Rounds);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
