import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { SmartHome } from '../../models/smart-home';
import { Router } from '@angular/router';

@Component({
  selector: 'app-smart-homes',
  templateUrl: './smart-homes.component.html',
  styleUrls: ['./smart-homes.component.scss']
})
export class SmartHomesComponent implements OnInit {

  public smartHomes: SmartHome[];
  
  constructor(
    private router: Router,
    private userService: UserService
  ) { }

  ngOnInit() {
    this.getSmartHomes();
  }

  getSmartHomes(){
    this.userService.getSmartHomes()
      .subscribe(x => this.smartHomes = x);
  }

  goToSmartHome(id){
    this.router.navigateByUrl('/rooms');
    this.userService.chooseSmartHome(id);
  }
  
}
