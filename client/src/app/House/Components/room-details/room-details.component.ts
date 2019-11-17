import { Component, OnInit, OnDestroy } from '@angular/core';
import { Chart } from 'chart.js';
import { RoomDetails } from '../../Models/room-details';
import { RoomsService } from '../../Services/rooms.service';
import { ActivatedRoute } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators'

@Component({
  selector: 'app-room-details',
  templateUrl: './room-details.component.html',
  styleUrls: ['./room-details.component.scss']
})
export class RoomDetailsComponent implements OnInit, OnDestroy {

  public lineChart = [];
  public roomDetails: RoomDetails;
  private _unsubscribe$: Subject<void> = new Subject<void>();

  constructor(
    private route: ActivatedRoute,
    private roomService: RoomsService
  ) {
    route.url.subscribe(_ => this.getRoom());
  }

  ngOnInit() {
    this.getRoom();
  }

  getRoom(): void {
    const id = this.route.snapshot.paramMap.get('id');
    this.roomService.getRoomDetails(id)
      .subscribe(roomDetails => {
        this.roomDetails = roomDetails;
        this._prepareChart();
      });
  }

  switchDevice(device, $event) {
    device.isOn = $event.target.checked;
    this.roomService.switchDevice(this.roomDetails.id, device)
      .pipe(takeUntil(this._unsubscribe$))
      .subscribe(x => this.roomDetails.devices.filter(y => y.id === x.id)[0] = x);
  }

  ngOnDestroy(): void {
    this._unsubscribe$.next();
    this._unsubscribe$.complete();
  }

  private _prepareChart(): void {
    if (this.roomDetails.sensorsHistory === undefined || this.roomDetails.sensorsHistory.length == 0) {
      this.lineChart = new Chart('lineChart');
    }
    else {
      this.lineChart = new Chart('lineChart', {
        type: 'line',
        data: {
          labels: ["0:00", "", "3:00", "", "6:00", "", "9:00", "", "12:00", "", "15:00", "", "18:00", "", "21:00", ""],
          datasets: [{
            label: 'Temperature',
            data: this.roomDetails.sensorsHistory.filter(function (sensorHistory) {
              return sensorHistory.type === 'Thermometer';
            })[0].historyEntries.map(historyEntry => historyEntry.value),
            fill: false,
            lineTension: 0.2,
            borderColor: "red",
            borderWidth: 1
          }, {
            label: 'Humidity',
            data: this.roomDetails.sensorsHistory.filter(function (sensorHistory) {
              return sensorHistory.type === 'HumiditySensor';
            })[0].historyEntries.map(historyEntry => historyEntry.value),
            fill: false,
            lineTension: 0.2,
            borderColor: "blue",
            borderWidth: 1
          }]
        },
        options: {
          title: {
            text: "Sensors",
            display: true
          },
          scales: {
            yAxes: [{
              ticks: {
                beginAtZero: true
              }
            }]
          }
        }
      });
    }
  }
}