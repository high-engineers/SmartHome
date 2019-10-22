import { Component, OnInit } from '@angular/core';
import { Chart } from 'chart.js';

@Component({
  selector: 'app-device-picker',
  templateUrl: './device-picker.component.html',
  styleUrls: ['./device-picker.component.scss']
})
export class DevicePickerComponent implements OnInit {

  constructor() { }
  LineChart = [];


  ngOnInit() {
    this.LineChart = new Chart('lineChart', {
      type: 'line',
      data: {
        labels: ["0:00", "3:00", "6:00", "9:00", "12:00", "15:00", "18:00", "21:00"],
        datasets: [{
          label: 'Temperature',
          data: [22, 20, 21, 23, 20, 20],
          fill: false,
          lineTension: 0.2,
          borderColor: "red",
          borderWidth: 1
        }, {
          label: 'Humidity',
          data: [8, 11, 12, 9, 11, 12],
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
