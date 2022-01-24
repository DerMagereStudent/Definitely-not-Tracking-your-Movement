package org.dermagerestudent.dntym

import android.content.Context
import android.content.SharedPreferences
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import androidx.fragment.app.Fragment
import org.dermagerestudent.dntym.constants.ApplicationConstants
import org.dermagerestudent.dntym.views.IdentityFragment
import org.dermagerestudent.dntym.views.TrackingFragment

class MainActivity : AppCompatActivity() {
    companion object {
        lateinit var instance: MainActivity
    }

    lateinit var sharedPreferences: SharedPreferences

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        this.setContentView(R.layout.activity_main)

        MainActivity.instance = this

        this.supportActionBar?.hide()

        this.sharedPreferences = this.applicationContext.getSharedPreferences(ApplicationConstants.sharedPreferencesKey, Context.MODE_PRIVATE)

        if (!this.sharedPreferences.contains(ApplicationConstants.authTokenPreferenceKey))
            this.sharedPreferences.edit().putString(ApplicationConstants.authTokenPreferenceKey, null).apply()

        if (this.sharedPreferences.getString(ApplicationConstants.authTokenPreferenceKey, null) == null)
            this.replaceFragment(IdentityFragment())
        else
            this.replaceFragment(TrackingFragment())
    }

    fun notifyPreferencesChanged() {
        // change fragment based on shared preference value (login status)
        if (MainActivity.instance.sharedPreferences.getString(ApplicationConstants.authTokenPreferenceKey, null) == null) {
            // goto identity fragment if no already there
            if (!this.isCurrentFragmentOfTag(IdentityFragment::class.java.name))
                this.replaceFragment(IdentityFragment())
        } else {
            // goto tracking fragment if not already there
            if (!this.isCurrentFragmentOfTag(TrackingFragment::class.java.name))
                this.replaceFragment(TrackingFragment())
        }
    }

    private fun replaceFragment(fragment: Fragment) {
        this.supportFragmentManager
            .beginTransaction()
            .setCustomAnimations(R.anim.fade_in, R.anim.fade_out)
            .replace(R.id.main_fragment_container, fragment, fragment.javaClass.name)
            .commit()
    }

    private fun isCurrentFragmentOfTag(tag: String): Boolean {
        val fragment = this.supportFragmentManager.findFragmentByTag(tag)
        return fragment != null && fragment.isVisible
    }
}