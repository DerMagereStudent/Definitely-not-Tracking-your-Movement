import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ToastrService } from 'ngx-toastr';

import { IdentityComponent } from './identity.component';
import { IdentityRoutingModule } from './identity-routing.module';
import { IdentitySignupComponent } from './components/identity-signup/identity-signup.component';
import { IdentityLoginComponent } from './components/identity-login/identity-login.component';
import { IdentityService } from './services/identity.service';

@NgModule({
  imports: [
    CommonModule,
    IdentityRoutingModule,
    FormsModule,
    ReactiveFormsModule,
  ],
  declarations: [
    IdentityComponent,
    IdentitySignupComponent,
    IdentityLoginComponent
  ],
  providers: [
    IdentityService,
    ToastrService
  ]
})
export class IdentityModule { }
