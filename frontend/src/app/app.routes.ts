import { Routes } from '@angular/router';

import { Circles } from './features/circles/circles';
import { CircleForm } from './features/circle-form/circle-form';

export const routes: Routes = [
  {
    path: 'circles',
    component: Circles
  },
  {
    path: 'circles/new',
    component: CircleForm
  },
  {
    path: '',
    redirectTo: 'circles',
    pathMatch: 'full'
  }
];