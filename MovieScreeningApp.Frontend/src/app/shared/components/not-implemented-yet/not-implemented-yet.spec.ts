import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NotImplementedYet } from './not-implemented-yet';

describe('NotImplementedYet', () => {
  let component: NotImplementedYet;
  let fixture: ComponentFixture<NotImplementedYet>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NotImplementedYet],
    }).compileComponents();

    fixture = TestBed.createComponent(NotImplementedYet);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
