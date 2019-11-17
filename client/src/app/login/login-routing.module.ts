import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WelcomeScreenComponent } from './components/welcome-screen/welcome-screen.component';
import { Routes, RouterModule } from '@angular/router';
import { SmartHomesComponent } from './components/smart-homes/smart-homes.component';

const routes: Routes = [
  {
    path: '',
    component: WelcomeScreenComponent,
  },
  {
    path: 'smartHomes',
    component: SmartHomesComponent,
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LoginRoutingModule { }
