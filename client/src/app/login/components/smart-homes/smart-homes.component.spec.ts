import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmartHomesComponent } from './smart-homes.component';

describe('SmartHomesComponent', () => {
  let component: SmartHomesComponent;
  let fixture: ComponentFixture<SmartHomesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmartHomesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmartHomesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
