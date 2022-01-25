import { style } from '@angular/animations';
import { Component, Input, OnInit } from '@angular/core';

import * as geojson from 'geojson';
import * as L from 'leaflet';

@Component({
  selector: 'app-tracking-map',
  templateUrl: './tracking-map.component.html',
  styleUrls: ['./tracking-map.component.scss']
})
export class TrackingMapComponent implements OnInit {

  private map: L.Map | null = null;
  private geojson: L.GeoJSON | null = null;

  private points: any[] | null = null

  @Input() set trackingPoints(value: any | null) {
    this.points = value;
    this.updateMap();
  }

  constructor() { }

  ngOnInit() {
    this.map = L.map('map', {
      center: [0, 0],
      maxBounds: [[-90, -180], [90, 180]],
      zoom: 3
    });

    const tiles = L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
      "maxZoom": 19,
      "minZoom": 3
    }).addTo(this.map);
  }

  async updateMap() {
    if (this.geojson != null) {
      this.geojson.remove()
      this.geojson = null;
    }

    if (this.points == null)
      return;

    var features: geojson.FeatureCollection<geojson.LineString> = <geojson.FeatureCollection<geojson.LineString>> {
      type: "FeatureCollection",
      features: []
    };

    for (var i = 0; i < this.points.length - 1; i++) {
      features.features.push(<geojson.Feature<geojson.LineString>>{
        type: "Feature",
        properties: {
          timeStampFirst: this.points[i].timeStampTracked,
          timeStampSecond: this.points[i + 1].timeStampTracked,
        },
        geometry: {
          type: "LineString",
          "coordinates": [
            [ this.points[i].longitude, this.points[i].latitude ],
            [ this.points[i + 1].longitude, this.points[i + 1].latitude ],
          ]
        }
      });
    }

    this.geojson = L.geoJSON(features, {
      style: (feature) => this.getVelocityColorStyle(feature)
    }).addTo(this.map!);
    this.map!.fitBounds(this.geojson.getBounds());
  }

  getVelocityColorStyle(feature: any): L.PathOptions {
    const distance = L.latLng(feature.geometry.coordinates[0][1], feature.geometry.coordinates[0][0])
                      .distanceTo(L.latLng(feature.geometry.coordinates[1][1], feature.geometry.coordinates[1][0]));

    const timeDifference = (feature.properties.timeStampSecond.getTime() - feature.properties.timeStampFirst.getTime()) / 1000.0;

    const speed = distance / timeDifference; // in m/s

    if (speed < 2) // ≈ 7 km/h
      return <L.PathOptions>{ color: '#3388ff', weight: 5 };
    
    if (speed < 7) // ≈ 25 km/h
      return <L.PathOptions>{ color: this.lerpColor([0x33, 0x88, 0xff], [0x00, 0x80, 0x00], this.mapNumber(speed, 2, 7, 0.0, 1.0)), weight: 5 };
    
    if (speed < 14) // ≈ 50 km/h
    return <L.PathOptions>{ color: this.lerpColor([0x00, 0x80, 0x00], [0xff, 0xff, 0x00], this.mapNumber(speed, 7, 14, 0.0, 1.0)), weight: 5 };
    
    if (speed < 28.8) // ≈ 100km/h
    return <L.PathOptions>{ color: this.lerpColor([0xff, 0xff, 0x00], [0xff, 0x00, 0x00], this.mapNumber(speed, 14, 28.8, 0.0, 1.0)), weight: 5 };
    
    return <L.PathOptions>{ color: '#ff0000', weight: 5 };
  }

  mapNumber(value: number, in_min: number, in_max: number, out_min: number, out_max: number): number {
    return (value - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
  }

  lerpColor(fromRGB: number[], toRGB: number[], by: number): string {
    if (by > 1)
      by = 1.0;
    else if (by < 0)
      by = 0.0

    const r = Math.floor(this.lerpNumber(fromRGB[0], toRGB[0], by));
    const g = Math.floor(this.lerpNumber(fromRGB[1], toRGB[1], by));
    const b = Math.floor(this.lerpNumber(fromRGB[2], toRGB[2], by));

    return `rgb(${r}, ${g}, ${b})`;
  }

  lerpNumber(x: number, y: number, a: number): number {
    return x * (1 - a) + y * a;
  }
}