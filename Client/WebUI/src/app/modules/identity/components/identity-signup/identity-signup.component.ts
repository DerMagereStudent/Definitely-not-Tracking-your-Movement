import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { IdentityService } from '../../services/identity.service';

@Component({
  selector: 'app-identity-signup',
  templateUrl: './identity-signup.component.html',
  styleUrls: ['./identity-signup.component.scss']
})
export class IdentitySignupComponent implements OnInit {
  formModel = this.fb.group({
    username: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    passwords: this.fb.group({
      password: ['', [Validators.required, Validators.minLength(6), Validators.pattern(/^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9\d]).{0,}$/)]],
      confirmPassword: ['', Validators.required]
    }, { validator: this.comparePasswords })

  });

  constructor(private router: Router, private fb: FormBuilder, private identityService: IdentityService, private toastrService: ToastrService) { }

  ngOnInit() {
    if (localStorage.getItem('token') != null)
      this.router.navigateByUrl('');
  }

  onSubmit() {
    this.identityService.sendSignUpRequest({ username: this.formModel.value.username, email: this.formModel.value.email, password: this.formModel.value.passwords.password})
      .subscribe({
        next: this.onSubmitSuccess.bind(this),
        error: (error) => { console.log(error) }
      })
  }

  onSubmitSuccess(response: any) {
    if (response.succeeded) {
      this.formModel.reset();
      response.messages.forEach((message: any) => {
        this.toastrService.success(message.description, message.code);
      });
    } else {
      response.errors.forEach((error: any) => {
        console.log(error);
        console.log(this.toastrService);

        this.toastrService.error(error.description, error.code);
      });
    }
  }

  comparePasswords(fb: FormGroup) {
    let confirmPswrdCtrl = fb.get('confirmPassword')!;
    
    if (confirmPswrdCtrl.errors == null || 'passwordMismatch' in confirmPswrdCtrl.errors) {
      if (fb.get('password')!.value != confirmPswrdCtrl.value)
        confirmPswrdCtrl.setErrors({ passwordMismatch: true });
      else
        confirmPswrdCtrl.setErrors(null);
    }
  }
}