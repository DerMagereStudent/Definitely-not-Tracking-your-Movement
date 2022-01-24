import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { IdentityService } from '../../services/identity.service';

@Component({
  selector: 'app-identity-login',
  templateUrl: './identity-login.component.html',
  styleUrls: ['./identity-login.component.scss']
})
export class IdentityLoginComponent implements OnInit {
  formModel: { usernameEmail: string, password: string } = {
    usernameEmail: '',
    password: ''
  }

  constructor(private router: Router, private identityService: IdentityService, private toastrService: ToastrService) { }

  ngOnInit() {
    if (localStorage.getItem('token') != null)
      this.router.navigateByUrl('');
  }

  onSubmit() {
    this.identityService.sendLoginRequest(this.formModel)
      .subscribe({
        next: this.onSubmitSuccess.bind(this),
        error: (error) => { console.log(error) }
      })
  }

  onSubmitSuccess(response: any) {
    if (response.succeeded) {
      localStorage.setItem('token', response.token);
      this.router.navigateByUrl('');
      response.messages.forEach((message: any) => {
        this.toastrService.success(message.description, message.code);
      });
    } else {
      response.errors.forEach((error: any) => {
        this.toastrService.error(error.description, error.code);
      });
    }
  } 
}