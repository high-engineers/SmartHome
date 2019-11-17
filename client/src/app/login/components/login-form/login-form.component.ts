import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { UserService } from '../../services/user.service';
import { LoginData } from '../../models/login-data';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login-form',
  templateUrl: './login-form.component.html',
  styleUrls: ['./login-form.component.scss']
})
export class LoginFormComponent implements OnInit {

  public username: string;
  public password: string;
  private loginData: LoginData;
  @Output() onClick = new EventEmitter();
  
  constructor(
    private router: Router,
    private userService: UserService
  ) { }

  ngOnInit() {
  }

  register(){
    this.onClick.emit();
  }

  login(){
    this.loginData = {
      username: this.username,
      password: this.password
    };

    this.userService.login(this.loginData)
      .subscribe(x => this.router.navigateByUrl('/smartHomes'));
  }

}
