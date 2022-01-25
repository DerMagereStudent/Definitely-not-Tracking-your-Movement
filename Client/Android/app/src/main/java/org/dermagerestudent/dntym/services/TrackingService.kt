package org.dermagerestudent.dntym.services

import android.Manifest
import android.app.*
import android.content.Context
import android.content.Intent
import android.content.pm.PackageManager
import android.graphics.Color
import android.location.Location
import android.os.Environment
import android.os.IBinder
import android.os.PowerManager
import android.util.Log
import android.widget.Toast
import androidx.annotation.NonNull
import androidx.core.app.ActivityCompat
import androidx.core.content.ContextCompat.getSystemService
import com.google.android.gms.location.LocationCallback
import com.google.android.gms.location.LocationRequest
import com.google.android.gms.location.LocationServices
import com.google.android.gms.tasks.CancellationToken
import com.google.android.gms.tasks.OnTokenCanceledListener
import com.google.android.gms.tasks.Task
import kotlinx.coroutines.*
import kotlinx.coroutines.tasks.await
import org.dermagerestudent.dntym.MainActivity
import org.dermagerestudent.dntym.R
import org.dermagerestudent.dntym.constants.ApplicationConstants
import org.dermagerestudent.dntym.entities.TrackingPoint
import org.dermagerestudent.dntym.networking.RetrofitInstance
import org.dermagerestudent.dntym.networking.contracts.requests.tracking.AddTrackingPointRequest
import java.io.File
import java.io.FileOutputStream
import java.io.PrintWriter
import java.lang.Exception
import java.util.*

class TrackingService : Service() {
    private val notificationId: Int = 1548732
    private val notificationChannelId: String = "TRACKING SERVICE CHANNEL"

    private lateinit var notification: Notification
    private lateinit var notificationBuilder: Notification.Builder
    private lateinit var notificationChannel: NotificationChannel
    private lateinit var notificationManager: NotificationManager

    enum class Actions {
        START,
        STOP
    }

    private var wakeLock: PowerManager.WakeLock? = null
    private var isServiceStarted = false

    override fun onBind(intent: Intent): IBinder? {
        Log.d("TrackingService","Some component want to bind with the service")
        // We don't provide binding, so return null
        return null
    }

    override fun onStartCommand(intent: Intent?, flags: Int, startId: Int): Int {
        Log.d("TrackingService", "onStartCommand executed with startId: $startId")
        if (intent != null) {
            val action = intent.action
            Log.d("TrackingService", "using an intent with action $action")
            when (action) {
                Actions.START.name -> startService()
                Actions.STOP.name -> stopService()
                else -> Log.d("TrackingService", "This should never happen. No action in the received intent")
            }
        } else {
            Log.d("TrackingService", 
                "with a null intent. It has been probably restarted by the system."
            )
        }
        // by returning this we make sure the service is restarted if the system kills the service
        return START_STICKY
    }

    override fun onCreate() {
        super.onCreate()
        Log.d("TrackingService", "The service has been created".toUpperCase())
        this.notification = createNotification()

        startForeground(this.notificationId, this.notification)
    }

    override fun onDestroy() {
        super.onDestroy()
        Log.d("TrackingService", "The service has been destroyed".toUpperCase())
        Toast.makeText(this, "Service destroyed", Toast.LENGTH_SHORT).show()
    }

    private fun startService() {
        if (isServiceStarted)
            return

        Log.d("TrackingService", "Starting the foreground service task")
        Toast.makeText(this, "Service starting its task", Toast.LENGTH_SHORT).show()
        isServiceStarted = true

        // we need this lock so our service gets not affected by Doze Mode
        wakeLock =
            (getSystemService(Context.POWER_SERVICE) as PowerManager).run {
                newWakeLock(PowerManager.PARTIAL_WAKE_LOCK, "EndlessService::lock").apply {
                    acquire(10*60*1000L /*10 minutes*/)
                }
            }

        // we're starting a loop in a coroutine
        CoroutineScope(Dispatchers.IO).launch {
            while (isServiceStarted) {
                delay(1000)

                val task = getLocationTask() ?: continue
                val location = task.await()
                val sharedPreferences = this@TrackingService.getSharedPreferences(ApplicationConstants.sharedPreferencesKey, Context.MODE_PRIVATE)

                if (sharedPreferences.getString(ApplicationConstants.authTokenPreferenceKey, null) == null)
                    continue;

                try {
                    val response = RetrofitInstance.api.sendAddTrackingPointRequest(
                        sharedPreferences.getString(ApplicationConstants.authTokenPreferenceKey, null)!!,
                        AddTrackingPointRequest(
                            null,
                            TrackingPoint(location.latitude, location.longitude, null, null)
                        )
                    )

                    if (response.isSuccessful && response.body()!!.succeeded)
                        this@TrackingService.updateNotificationText("${location.latitude}, ${location.longitude}")
                    else
                        this@TrackingService.updateNotificationText("error ${Date().time}")
                } catch (e: Exception) {
                    Log.e("TrackingService", e.message!!)
                }
            }

            Log.d("TrackingService", "End of the loop for the service")
        }
    }

    private fun stopService() {
        Log.d("TrackingService", "Stopping the foreground service")
        Toast.makeText(this, "Service stopping", Toast.LENGTH_SHORT).show()
        try {
            wakeLock?.let {
                if (it.isHeld) {
                    it.release()
                }
            }
            stopForeground(true)
            stopSelf()
        } catch (e: Exception) {
            Log.d("TrackingService", "Service stopped without being started: ${e.message}")
        }
        isServiceStarted = false
    }

    private suspend fun getLocationTask(): Task<Location>? {
        if (ActivityCompat.checkSelfPermission(this, Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED &&
            ActivityCompat.checkSelfPermission(this, Manifest.permission.ACCESS_COARSE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
            // TODO: Consider calling
            //    ActivityCompat#requestPermissions
            // here to request the missing permissions, and then overriding
            //   public void onRequestPermissionsResult(int requestCode, String[] permissions,
            //                                          int[] grantResults)
            // to handle the case where the user grants the permission. See the documentation
            // for ActivityCompat#requestPermissions for more details.
            return null
        }

        return LocationServices.getFusedLocationProviderClient(MainActivity.instance)
            .getCurrentLocation(LocationRequest.PRIORITY_HIGH_ACCURACY, object: CancellationToken() {
                override fun isCancellationRequested(): Boolean {
                    return false
                }

                @NonNull
                override fun onCanceledRequested(@NonNull onTokenCanceledListener: OnTokenCanceledListener): CancellationToken {
                    return this
                }
            })
    }

    private fun createNotification(): Notification {
        // depending on the Android API that we're dealing with we will have
        // to use a specific method to create the notification
        this.notificationManager = getSystemService(Context.NOTIFICATION_SERVICE) as NotificationManager;
        this.notificationChannel = NotificationChannel(
            this.notificationChannelId,
            "Tracking Service notifications channel",
            NotificationManager.IMPORTANCE_LOW
        ).let {
            it.description = "Tracking Service channel"
            it.enableLights(true)
            it.lightColor = Color.RED
            it.enableVibration(false)
            it
        }
        this.notificationManager.createNotificationChannel(this.notificationChannel)

        val pendingIntent: PendingIntent = Intent(this, MainActivity::class.java).let { notificationIntent ->
            PendingIntent.getActivity(this, 0, notificationIntent, 0)
        }

        this.notificationBuilder = Notification.Builder(
            this,
            this.notificationChannelId
        )

        return this.notificationBuilder
            .setContentTitle("Tracking Service")
            .setContentText("This service is definitely not tracking your movement")
            .setContentIntent(pendingIntent)
            .setSmallIcon(R.mipmap.ic_launcher)
            .setTicker("Ticker text")
            .build()
    }

    private fun updateNotificationText(text: String) {
        this.notificationBuilder.setContentText(text)
        this.notification = this.notificationBuilder.build()
        this.notificationManager.notify(this.notificationId, this.notification)
    }
}