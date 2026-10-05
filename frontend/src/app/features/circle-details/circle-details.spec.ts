import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CircleDetails } from './circle-details';

describe('CircleDetails', () => {
  let component: CircleDetails;
  let fixture: ComponentFixture<CircleDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CircleDetails],
    }).compileComponents();

    fixture = TestBed.createComponent(CircleDetails);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
