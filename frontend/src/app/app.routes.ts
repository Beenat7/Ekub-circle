import { Routes } from '@angular/router';
import { Login } from './auth/login/login';
import { Register } from './auth/register/register';
import { Dashboard } from './dashboard/dashboard';
import { Payment } from './payment/payment';
import { Payout } from './payout/payout';
import { Rounds } from './rounds/rounds';
export const routes: Routes = [
  {
    path: 'login',
    component: Login
  },

  {
    path: 'register',
    component: Register
  },

  {
    path: 'dashboard',
    component: Dashboard
  },
  {
  path: 'payment',
  component: Payment
},
{
  path: 'payout',
  component: Payout
},
{
  path: 'rounds',
  component: Rounds
},

  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  }
];