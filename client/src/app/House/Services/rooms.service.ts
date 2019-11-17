import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Room } from '../Models/room';
import { RoomDetails } from '../Models/room-details';
import { Device } from '../Models/device';
import { DeviceDetails } from '../Models/device-details';
import { NewRoom } from '../Models/new-room';
import { UserService } from '../../login/services/user.service';
import { environment } from '../../../environments/environment.prod';

@Injectable({
  providedIn: 'root'
})
export class RoomsService {
  private urlBase = environment.apiUrl;
  private roomsUrl = this.urlBase + 'api/rooms';

  httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
  };

  constructor(
    private userService: UserService,
    private http: HttpClient
  ) { }

  getRooms(): Observable<Room[]>{
    return this.http.get<Room[]>(this.roomsUrl + this.userService.getUserParams());
  }

  getRoomDetails(id: string): Observable<RoomDetails>{
    return this.http.get<RoomDetails>(`${this.roomsUrl}/${id}${this.userService.getUserParams()}`);
  }

  switchDevice(roomId: string, device: DeviceDetails): Observable<DeviceDetails>{
    return this.http.put<DeviceDetails>(`${this.roomsUrl}/${roomId}/components/${device.id}${this.userService.getUserParams()}&newState=${device.isOn}`, this.httpOptions);
  }

  addRoom(newRoom: NewRoom){
    return this.http.post<NewRoom>(`${this.roomsUrl}add${this.userService.getUserParams()}`, newRoom, this.httpOptions);
  }
}
