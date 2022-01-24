import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class IdentityService {
  readonly BaseURI = 'http://api.definitely-not-tracking-your-movement.com/api';

  constructor(private http: HttpClient) { }

  sendSignUpRequest(body: { username: string, email: string, password: string }) {
    return this.http.post(this.BaseURI + '/identity/signup', body)
  }

  sendLoginRequest(body: { usernameEmail: string, password: string }) {
    return this.http.post(this.BaseURI + '/identity/login', body)
  }
}