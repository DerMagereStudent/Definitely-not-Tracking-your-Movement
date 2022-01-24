import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  { path: '', redirectTo: 'tracking', pathMatch: 'full'},
  { path:'identity', loadChildren: () => import('./modules/identity/identity.module').then(m => m.IdentityModule) },
  { path:'tracking', loadChildren: () => import('./modules/tracking/tracking.module').then(m => m.TrackingModule) },
  { path: '**', redirectTo: 'tracking', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }