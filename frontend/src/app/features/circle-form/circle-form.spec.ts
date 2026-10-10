import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { CircleForm } from './circle-form';
import { CircleService } from '../../services/circle';

describe('CircleForm', () => {
  let component: CircleForm;
  let fixture: ComponentFixture<CircleForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CircleForm],
      providers: [
        provideRouter([]),
        { provide: CircleService, useValue: { create: () => of({}) } }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CircleForm);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
