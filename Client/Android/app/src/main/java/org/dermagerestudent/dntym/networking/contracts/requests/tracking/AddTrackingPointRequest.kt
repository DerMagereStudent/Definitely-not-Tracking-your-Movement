package org.dermagerestudent.dntym.networking.contracts.requests.tracking

import org.dermagerestudent.dntym.entities.TrackingPoint

data class AddTrackingPointRequest(
    val userId: String?,
    val trackingPoint: TrackingPoint
)
