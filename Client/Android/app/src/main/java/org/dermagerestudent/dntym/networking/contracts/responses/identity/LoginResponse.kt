package org.dermagerestudent.dntym.networking.contracts.responses.identity

import org.dermagerestudent.dntym.valueobjects.Message

data class LoginResponse(
    val succeeded: Boolean,
    val token: String,
    val messages: List<Message>,
    val errors: List<Message>
)
