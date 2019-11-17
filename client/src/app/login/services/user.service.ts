import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { NewUser } from '../models/new-user';
import { LoginData } from '../models/login-data';
import { LoggedUser } from '../models/logged-user';
import { SmartHome } from '../models/smart-home';
import { SelectedSmartHome } from '../models/selected-smart-home';
import { environment } from '../../../environments/environment.prod';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private urlBase = environment.apiUrl;
  private usersUrl = this.urlBase + 'api/users';

  private loggedUser: LoggedUser;
  private selectedSmartHome: SelectedSmartHome;

  httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
  };
  
  constructor(private http: HttpClient) { }

  register(user: NewUser){
    return this.http.post<NewUser>(`${this.usersUrl}/register`, user, this.httpOptions);
  }

  login(loginData: LoginData): Observable<LoggedUser>{
    let result = this.http.post<LoggedUser>(`${this.usersUrl}/login`, loginData, this.httpOptions);
    result.subscribe(x => this.loggedUser = x);
    return result;
  }

  getSmartHomes(){
    return this.http.get<SmartHome[]>(`${this.usersUrl}/smartHomes?requestedByUserId=${this.loggedUser.id}`);
  }

  chooseSmartHome(smartHomeId: string){
    this.selectedSmartHome = {
      id: smartHomeId
    };
  }

  getUserParams(){
    return `?requestedByUserId=${this.loggedUser.id}&smartHomeEntityId=${this.selectedSmartHome.id}`;
  }
}
