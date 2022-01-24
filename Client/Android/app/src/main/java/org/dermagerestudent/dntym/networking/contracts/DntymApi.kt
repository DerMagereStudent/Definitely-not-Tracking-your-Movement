package org.dermagerestudent.dntym.networking.contracts

import org.dermagerestudent.dntym.networking.contracts.requests.identity.LoginRequest
import org.dermagerestudent.dntym.networking.contracts.requests.identity.SignUpRequest
import org.dermagerestudent.dntym.networking.contracts.requests.tracking.AddTrackingPointRequest
import org.dermagerestudent.dntym.networking.contracts.responses.identity.LoginResponse
import org.dermagerestudent.dntym.networking.contracts.responses.identity.SignUpResponse
import org.dermagerestudent.dntym.networking.contracts.responses.tracking.AddTrackingPointResponse

import retrofit2.Response
import retrofit2.http.*

interface DntymApi {
    @POST("api/identity/signup")
    suspend fun sendSignUpRequest(@Body request: SignUpRequest) : Response<SignUpResponse?>

    @POST("api/identity/login")
    suspend fun sendLoginRequest(@Body request: LoginRequest) : Response<LoginResponse?>

    @PUT("api/tracking/add-position")
    suspend fun sendAddTrackingPointRequest(@Header("Authorization") token: String, @Body request: AddTrackingPointRequest) : Response<AddTrackingPointResponse?>
}