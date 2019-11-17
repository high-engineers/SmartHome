import { Component, OnInit } from '@angular/core';
import { UnconecttedDevice } from '../../Models/unconectted-device';
import { ComponentService } from '../../Services/component.service';
import { DeviceDetails } from '../../Models/device-details';
import { ActivatedRoute } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/internal/operators/takeUntil';

@Component({
  selector: 'app-add-device',
  templateUrl: './add-device.component.html',
  styleUrls: ['./add-device.component.scss']
})
export class AddDeviceComponent implements OnInit {

  public name: string;
  public selectedDevice: UnconecttedDevice;
  public unconnectedDevices: UnconecttedDevice[];
  private deviceDetails: DeviceDetails;
  private _unsubscribe$: Subject<void> = new Subject<void>();

  constructor(
    private componentService: ComponentService,
    private route: ActivatedRoute,
  ) { }

  ngOnInit() {
    this.getUnconnectedComponents();
  }

  private getUnconnectedComponents() {
    this.componentService.getUnconnectedComponents()
      .subscribe(x => this.unconnectedDevices = x);
  }

  public addDevice(): void {
    const roomId = this.route.snapshot.paramMap.get('id');
    
    this.deviceDetails = {
      id: this.selectedDevice.id,
      name: this.name,
      type: this.selectedDevice.type,
      isOn: false
    };
    this.componentService.connectComponent(this.deviceDetails, roomId)
    .pipe(takeUntil(this._unsubscribe$))
    .subscribe();
  }

  ngOnDestroy(): void {
    this._unsubscribe$.next();
    this._unsubscribe$.complete();
  }
  
}
