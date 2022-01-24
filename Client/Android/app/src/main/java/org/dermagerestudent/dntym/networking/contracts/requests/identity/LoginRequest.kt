package org.dermagerestudent.dntym.networking.contracts.requests.identity

data class LoginRequest(
    val usernameEmail: String,
    val password: String
)