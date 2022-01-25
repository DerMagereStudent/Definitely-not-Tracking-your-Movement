import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

import { TrackingService } from './services/tracking.service';

@Component({
  selector: 'app-tracking',
  templateUrl: './tracking.component.html',
  styleUrls: ['./tracking.component.scss']
})
export class TrackingComponent implements OnInit {
  trackingPoints: any[] | null = null;

  constructor(private router: Router, private trackingService: TrackingService, private toastrService: ToastrService) { }

  ngOnInit() {
    this.trackingService.sendGetPositionsRequest(localStorage.getItem('token')!).subscribe({
      next: (response: any) => {
        if (response.succeeded) {
          this.trackingPoints = (response.trackingPoints as any[])
            .sort((a,b) => new Date(a.timeStampTracked).getTime() - new Date(b.timeStampTracked).getTime())
            .map(p => { p.timeStampTracked = new Date(p.timeStampTracked); p.address = JSON.parse(p.address); return p });

          response.messages.forEach((message: any) => {
            this.toastrService.success(message.description, message.code);
          });
        } else {
          response.errors.forEach((error: any) => {
            this.toastrService.error(error.description, error.code);
          });
        }
      }
    })
  }

  onClick() {
    localStorage.removeItem('token');
    this.router.navigateByUrl('');
  }
}
