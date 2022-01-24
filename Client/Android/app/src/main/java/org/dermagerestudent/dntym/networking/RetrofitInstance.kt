package org.dermagerestudent.dntym.networking

import org.dermagerestudent.dntym.networking.contracts.DntymApi
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

object RetrofitInstance {
    private const val baseUrl: String = "http://h2950607.stratoserver.net/"
    // private const val baseUrl: String = "http://definitely-not-tracking-your-movement.com"

    private val retrofit by lazy {
        Retrofit.Builder()
            .baseUrl(this.baseUrl)
            .addConverterFactory(GsonConverterFactory.create())
            .build()
    }

    val api: DntymApi by lazy {
        this.retrofit.create(DntymApi::class.java)
    }
}