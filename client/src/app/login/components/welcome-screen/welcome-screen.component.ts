import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-welcome-screen',
  templateUrl: './welcome-screen.component.html',
  styleUrls: ['./welcome-screen.component.scss']
})
export class WelcomeScreenComponent implements OnInit {

  public login: boolean;
  
  constructor() { }

  ngOnInit() {
    this.login = true;
  }

  public handleSwitch(){
    this.login = !this.login;
  }
  
}
