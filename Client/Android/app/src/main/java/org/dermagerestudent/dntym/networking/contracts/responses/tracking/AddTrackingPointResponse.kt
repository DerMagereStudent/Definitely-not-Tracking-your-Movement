package org.dermagerestudent.dntym.networking.contracts.responses.tracking

import org.dermagerestudent.dntym.valueobjects.Message

data class AddTrackingPointResponse(
    val succeeded: Boolean,
    val messages: List<Message>,
    val errors: List<Message>
)