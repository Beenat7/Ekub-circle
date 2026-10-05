import { Routes } from '@angular/router';

import { Login } from './auth/login/login';
import { Register } from './auth/register/register';
import { Dashboard } from './dashboard/dashboard';
import { Payment } from './payment/payment';
import { Payout } from './payout/payout';
import { Rounds } from './rounds/rounds';

import { Circles } from './features/circles/circles';
import { CircleForm } from './features/circle-form/circle-form';
import { CircleDetails } from './features/circle-details/circle-details';
import { Members } from './features/members/members';

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
    path: 'circles/new',
    component: CircleForm
  },
  {
    path: 'circles/:id',
    component: CircleDetails
  },
  {
    path: 'circles',
    component: Circles,
    pathMatch: 'full'
  },
  {
    path: 'members',
    component: Members
  },
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  }
];