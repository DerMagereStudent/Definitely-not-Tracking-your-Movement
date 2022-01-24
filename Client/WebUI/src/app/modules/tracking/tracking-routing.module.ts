import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuardService } from './services/auth-guard.service';
import { TrackingComponent } from './tracking.component';

const routes: Routes = [
  { path: '', component: TrackingComponent, canActivate: [AuthGuardService] },
];

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule],
  providers: [AuthGuardService]
})
export class TrackingRoutingModule { }