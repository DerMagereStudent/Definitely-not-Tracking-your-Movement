import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TrackingService {
  readonly BaseURI = 'http://api.definitely-not-tracking-your-movement.com/api';

  constructor(private http: HttpClient) { }

  sendGetPositionsRequest(token: string) {
    return this.http.get(this.BaseURI + '/tracking/positions', { headers: { Authorization: 'Bearer ' + token }})
  }

}
