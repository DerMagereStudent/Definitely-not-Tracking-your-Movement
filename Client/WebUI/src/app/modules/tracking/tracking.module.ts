import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TrackingComponent } from './tracking.component';
import { TrackingMapComponent } from './components/tracking-map/tracking-map.component';
import { TrackingTimelineComponent } from './components/tracking-timeline/tracking-timeline.component';
import { TrackingRoutingModule } from './tracking-routing.module';

@NgModule({
  imports: [
    CommonModule,
    TrackingRoutingModule
  ],
  declarations: [
    TrackingComponent,
    TrackingMapComponent,
    TrackingTimelineComponent
  ]
})
export class TrackingModule { }
