import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DevicePickerComponent } from './device-picker.component';

describe('DevicePickerComponent', () => {
  let component: DevicePickerComponent;
  let fixture: ComponentFixture<DevicePickerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DevicePickerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DevicePickerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
