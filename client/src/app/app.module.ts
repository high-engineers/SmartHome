import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { RoomPickerComponent } from './House/Components/room-picker/room-picker.component';
import { DevicePickerComponent } from './House/Components/device-picker/device-picker.component';
import { RoomComponent } from './House/Components/room/room.component';

@NgModule({
  declarations: [
    AppComponent,
    RoomPickerComponent,
    DevicePickerComponent,
    RoomComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
