import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-tracking-timeline',
  templateUrl: './tracking-timeline.component.html',
  styleUrls: ['./tracking-timeline.component.scss']
})
export class TrackingTimelineComponent implements OnInit {
  private minDurationForVisit = 8 * 60;

  private points: any[] | null = null;

  timelineEntities: { date: string, header: string, content: string}[] = [];

  @Input() set trackingPoints(value: any | null) {
    this.points = value;
    this.updateTimeline();
  }

  constructor() { }

  ngOnInit() {
  }

  updateTimeline() {
    if (this.points == null || this.points.length == 0)
      return;

    var groups: any[][] = this.groupAdjecentPoints();
    var visitGroups: number[] = this.getVisitGroups(groups);

    // this.tryCombineNonContinuousVisits(groups, visitGroups);

    this.placeTimelineEntities(groups, visitGroups);
  }

  groupAdjecentPoints(): any[][] {
    var groups: any[][] = [[this.points![0]]];

    for (var i = 1; i < this.points!.length; i++) {
      if (groups.slice(-1)[0].slice(-1)[0].address.features[0].properties.place_id === this.points![i].address.features[0].properties.place_id)
        groups.slice(-1)[0].push(this.points![i]);
      else
        groups.push([this.points![i]]);
    }

    return groups;
  }

  getVisitGroups(groups: any[][]): number[] {
    var visitGroups: number[] = [];
    groups.forEach((group: any, index: number) => {
      var time = this.getElapsedTimeInGroupInSeconds(group);

      if (time > this.minDurationForVisit)
        visitGroups.push(index);
    });

    return visitGroups;
  }

  tryCombineNonContinuousVisits(groups: any[][], visitGroups: number[]) {
    while(true) {
      var intermediateRanges: number[][] = this.getIntermediateGroupRanges(groups.length, visitGroups);

      var newInterruptedVisit: boolean = false;

      for (var i = intermediateRanges.length - 1; i >= 0; i--) {
        var interruptedVisit: number[] | null = this.getInterruptedVisitRange(groups, intermediateRanges[0]);

        if (interruptedVisit == null)
          continue;

        newInterruptedVisit = true;

        var index: number = interruptedVisit[0];
        var count: number = interruptedVisit[1] - index + 1;

        var values: any[][] = groups.slice(interruptedVisit[0], interruptedVisit[1] + 1);
        var combinedValues: any[] = values.flat(1);

        groups.splice(index, count);
        groups.splice(index, 0, combinedValues);

        for (var j = 0; j < visitGroups.length; j++) {
          if (visitGroups[j] > index)
            visitGroups[j] -= count - 1;
        }

        visitGroups.push(index);
        visitGroups.sort();
      }

      if (!newInterruptedVisit)
        break;
    }

    for (var i = 0; i < visitGroups.length; i++) {
      console.log(groups[visitGroups[i]][0].address.features[0].properties.display_name);
    }
  }

  // gets the including ranges of indices between visit groups which contain movement data or interrupted visits
  getIntermediateGroupRanges(groupsLength: number, visitGroups: number[]): number[][] {
    if (visitGroups.length === 0)
      return [];

    var intermediateRanges: number[][] = [];

    if (visitGroups[0] > 0)
      intermediateRanges.push([0, visitGroups[0] - 1]) // all groups before the first visit group

    for (var i = 0; i < visitGroups.length - 1; i++) {
      if (visitGroups[i + 1] - visitGroups[i] <= 1)
        continue;

      intermediateRanges.push([visitGroups[i] + 1, visitGroups[i + 1] - 1])
    }

    if (visitGroups.slice(-1)[0] < groupsLength - 1)
      intermediateRanges.push([visitGroups.slice(-1)[0] + 1, groupsLength - 1]);

    return intermediateRanges;
  }

  getInterruptedVisitRange(groups: any[][], range: number[]): number[] | null {
    if (range[0] === range[1]) // only one module
      return null // not big enough otherwise it would already be a visit group

    var addressPointCounts: { placeId: number, count: number}[] = []

    for (var i = range[0]; i <= range[1]; i++) {
      const placeId = groups[i][0].address.features[0].properties.place_id;

      if (addressPointCounts.findIndex(v => v.placeId === placeId) < 0)
        addressPointCounts.push({ placeId: placeId, count: 0});

      addressPointCounts.find(v => v.placeId === placeId)!.count++;
    }

    var maxVisit: {range: number[], pointsCount: number} | null = null

    for (var placeIdObj of addressPointCounts) {
      if (placeIdObj.count < 2)
        continue;
      
      var found: number = 0;
      var foundIndex: number = 0;
      var visitTime: number = 0;
      var interruptTime: number = 0;
      var pointCount: number = 0;

      for (var i = range[0]; i <= range[1] && found < placeIdObj.count; i++) {
        if (found > 0)
          pointCount += groups[i].length;

        if (placeIdObj.placeId === groups[i][0].address.features[0].properties.place_id) {
          if (found === 0) {
            foundIndex = i;
            pointCount += groups[i].length;
          }

          visitTime += this.getElapsedTimeInGroupInSeconds(groups[i]);
          found++;

          if (found > 1) {
            if (visitTime >= interruptTime && visitTime + interruptTime > this.minDurationForVisit) {
              if (maxVisit == null)
                maxVisit = { range: [foundIndex, i], pointsCount: pointCount }
              else if (maxVisit.pointsCount < pointCount)
                maxVisit = { range: [foundIndex, i], pointsCount: pointCount }
            }
          }
        } else {
          if (found > 0)
            interruptTime += this.getElapsedTimeInGroupInSeconds(groups[i]);
        }
      }
    }

    return maxVisit == null ? null : maxVisit!.range;
  }

  getElapsedTimeInGroupInSeconds(group: any[]): number {
    if (group.length < 2)
      return 0;
    
    return (group.slice(-1)[0].timeStampTracked.getTime() - group[0].timeStampTracked.getTime()) / 1000
  }

  placeTimelineEntities(groups: any[][], visitGroups: number[]) {
    this.timelineEntities = [];

    if (visitGroups.length === 0) {
      this.timelineEntities.push({
        date: groups[0][0].timeStampTracked.toUTCString() + " - " + groups.slice(-1)[0].slice(-1)[0].timeStampTracked.toUTCString(),
        header: "On the way",
        content: ""
      });
      return;
    }

    if (visitGroups[0] > 0) {
      this.timelineEntities.push({
        date: groups[0][0].timeStampTracked.toUTCString() + " - " + groups[visitGroups[0] - 1].slice(-1)[0].timeStampTracked.toUTCString(),
        header: "On the way",
        content: ""
      });
    }

    for (var i = 0; i < visitGroups.length - 1; i++) {
      this.timelineEntities.push({
        date: groups[visitGroups[i]][0].timeStampTracked.toUTCString() + " - " + groups[visitGroups[i]].slice(-1)[0].timeStampTracked.toUTCString(),
        header: "Visit",
        content: groups[visitGroups[i]][0].address.features[0].properties.display_name
      });

      if (visitGroups[i + 1] - visitGroups[i] <= 1)
        continue;

      this.timelineEntities.push({
        date: groups[visitGroups[i] + 1][0].timeStampTracked.toUTCString() + " - " + groups[visitGroups[i + 1] - 1].slice(-1)[0].timeStampTracked.toUTCString(),
        header: "On the way",
        content: ""
      });
    }

    this.timelineEntities.push({
      date: groups[visitGroups.slice(-1)[0]][0].timeStampTracked.toUTCString() + " - " + groups[visitGroups.slice(-1)[0]].slice(-1)[0].timeStampTracked.toUTCString(),
      header: "Visit",
      content: groups[visitGroups[i]][0].address.features[0].properties.display_name
    });

    if (visitGroups.slice(-1)[0] < groups.length - 1) {
      this.timelineEntities.push({
        date: groups[visitGroups.slice(-1)[0] + 1][0].timeStampTracked.toUTCString() + " - " + groups[groups.length - 1].slice(-1)[0].timeStampTracked.toUTCString(),
        header: "On the way",
        content: ""
      });
    }
  }
}
