import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CircleForm } from './circle-form';

describe('CircleForm', () => {
  let component: CircleForm;
  let fixture: ComponentFixture<CircleForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CircleForm],
    }).compileComponents();

    fixture = TestBed.createComponent(CircleForm);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
