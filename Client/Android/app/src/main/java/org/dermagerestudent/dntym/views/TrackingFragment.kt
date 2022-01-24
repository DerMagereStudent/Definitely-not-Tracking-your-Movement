package org.dermagerestudent.dntym.views

import android.content.Context
import android.content.Intent
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.TextView
import androidx.core.content.ContextCompat.startForegroundService
import org.dermagerestudent.dntym.MainActivity
import org.dermagerestudent.dntym.R
import org.dermagerestudent.dntym.constants.ApplicationConstants
import org.dermagerestudent.dntym.services.TrackingService

class TrackingFragment : Fragment() {
    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View? {
        // Inflate the layout for this fragment
        val view = inflater.inflate(R.layout.fragment_tracking, container, false)

        val textViewLogout = view.findViewById<TextView>(R.id.text_view_logout)
        val buttonSend = view.findViewById<Button>(R.id.button_send)

        textViewLogout.setOnClickListener {
            MainActivity.instance.sharedPreferences.edit()
                .putString(ApplicationConstants.authTokenPreferenceKey, null)
                .apply()

            MainActivity.instance.notifyPreferencesChanged()
        }

        buttonSend.setOnClickListener {
            Intent(this.context, TrackingService::class.java).also {
                it.action = TrackingService.Actions.START.name
                startForegroundService(this.requireContext(), it)
            }
        }

        return view
    }


}