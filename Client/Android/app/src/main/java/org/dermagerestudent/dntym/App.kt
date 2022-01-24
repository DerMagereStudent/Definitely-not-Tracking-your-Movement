package org.dermagerestudent.dntym

import android.app.Application
import android.app.NotificationChannel
import android.app.NotificationManager

class App : Application() {
    companion object {
        const val trackingServiceChannelId = "trackingServiceChannelId"
    }

    override fun onCreate() {
        super.onCreate()

        val trackingServiceChannel = NotificationChannel(
            App.trackingServiceChannelId,
            "Tracking Service Channel",
            NotificationManager.IMPORTANCE_DEFAULT
        );

        val notificationManager: NotificationManager = getSystemService(NotificationManager::class.java)
        notificationManager.createNotificationChannel(trackingServiceChannel)
    }
}