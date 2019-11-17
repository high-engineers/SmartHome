import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AddDeviceComponent } from './Components/add-device/add-device.component';
import { AddRoomComponent } from './Components/add-room/add-room.component';
import { NavbarComponent } from './Components/navbar/navbar.component';
import { RoomDetailsComponent } from './Components/room-details/room-details.component';
import { HouseRoutingModule } from './house-routing.module';
import { HomeComponent } from './Components/home/home.component';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    AddDeviceComponent,
    AddRoomComponent,
    NavbarComponent,
    RoomDetailsComponent,
    HomeComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    HouseRoutingModule
  ]
})
export class HouseModule { }
