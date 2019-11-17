import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginFormComponent } from './components/login-form/login-form.component';
import { RegisterFormComponent } from './components/register-form/register-form.component';
import { WelcomeScreenComponent } from './components/welcome-screen/welcome-screen.component';
import { LoginRoutingModule } from './login-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SmartHomesComponent } from './components/smart-homes/smart-homes.component';

@NgModule({
  declarations: [
    LoginFormComponent, 
    RegisterFormComponent,
    WelcomeScreenComponent,
    SmartHomesComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LoginRoutingModule
  ]
})
export class LoginModule { }
