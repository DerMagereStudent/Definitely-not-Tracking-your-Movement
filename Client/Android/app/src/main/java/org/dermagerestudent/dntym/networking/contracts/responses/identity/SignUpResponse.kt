package org.dermagerestudent.dntym.networking.contracts.responses.identity

import org.dermagerestudent.dntym.valueobjects.Message

data class SignUpResponse(
    val succeeded: Boolean,
    val messages: List<Message>,
    val errors: List<Message>
)
