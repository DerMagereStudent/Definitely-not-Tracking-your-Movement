package org.dermagerestudent.dntym.entities

import java.util.*

data class TrackingPoint(
    val latitude: Double,
    val longitude: Double,
    val timeStampTracked: Date?,
    val address: String?
)
