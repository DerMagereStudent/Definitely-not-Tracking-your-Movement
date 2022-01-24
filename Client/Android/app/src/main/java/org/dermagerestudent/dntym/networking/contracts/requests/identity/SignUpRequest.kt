package org.dermagerestudent.dntym.networking.contracts.requests.identity

data class SignUpRequest(
    val username: String,
    val email: String,
    val password: String
)
