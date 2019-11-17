import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../../services/user.service';
import { NewUser } from '../../models/new-user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register-form',
  templateUrl: './register-form.component.html',
  styleUrls: ['./register-form.component.scss']
})
export class RegisterFormComponent implements OnInit {

  public registerForm: FormGroup;
  public submitted = false;
  public notMatch = false;
  private newUser: NewUser;

  @Output() onClick = new EventEmitter();

  constructor(
    private router: Router,
    private formBuilder: FormBuilder,
    private userService: UserService
  ) { }

  ngOnInit() {
    this.registerForm = this.formBuilder.group({
      username: ['', Validators.required],
      email: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', []]
    });
  }

  public login() {
    this.onClick.emit();
  }

  public get f() { return this.registerForm.controls; }

  public onSubmit() {
    this.submitted = true;
    if (this.registerForm.invalid) {
      return;
    }

    let password = this.registerForm.get('password').value;
    let confirmPassword = this.registerForm.get('confirmPassword').value;

    if(password !== confirmPassword){
      this.notMatch = true;
      return;
    }
    
    this.notMatch = false;
    
    this.newUser = {
      username: this.registerForm.get('username').value,
      email: this.registerForm.get('email').value,
      password: password
    };
    
    this.userService.register(this.newUser)
      .subscribe(x => {
        this.newUser = x;
        this.router.navigateByUrl('/smartHomes');
      });
  }
}
