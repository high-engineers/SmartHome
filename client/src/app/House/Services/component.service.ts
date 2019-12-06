import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { UnconecttedDevice } from '../Models/unconectted-device';
import { Observable } from 'rxjs';
import { Device } from '../Models/device';
import { DeviceDetails } from '../Models/device-details';
import { UserService } from '../../login/services/user.service';
import { environment } from '../../../environments/environment.prod';

@Injectable({
  providedIn: 'root'
})
export class ComponentService {

  private urlBase = environment.apiUrl;
  private componentsUrl = this.urlBase + 'api/components/';

  httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
  };

  constructor(
    private userService: UserService,
    private http: HttpClient
  ) { }

  getUnconnectedComponents(): Observable<UnconecttedDevice[]> {
    return this.http.get<UnconecttedDevice[]>(this.componentsUrl + 'getUnconnected' + this.userService.getUserParams());
  }

  connectComponent(component: DeviceDetails, roomId: string){
    return this.http.put(this.componentsUrl + component.id + 
      this.userService.getUserParams() + '&name=' + component.name + '&roomId=' + roomId, {}, this.httpOptions);
  }

}
