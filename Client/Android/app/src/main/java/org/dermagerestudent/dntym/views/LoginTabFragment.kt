package org.dermagerestudent.dntym.views

import android.content.Context
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.EditText
import android.widget.Toast
import androidx.fragment.app.Fragment
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.GlobalScope
import kotlinx.coroutines.launch
import org.dermagerestudent.dntym.MainActivity

import org.dermagerestudent.dntym.R
import org.dermagerestudent.dntym.constants.ApplicationConstants
import org.dermagerestudent.dntym.networking.RetrofitInstance
import org.dermagerestudent.dntym.networking.contracts.requests.identity.LoginRequest

class LoginTabFragment : Fragment() {
    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View? {
        // Inflate the layout for this fragment
        val view: View = inflater.inflate(R.layout.fragment_login_tab, container, false)

        val editTextUsernameEmail = view.findViewById<EditText>(R.id.edit_text_login_username_email)!!
        val editTextPassword = view.findViewById<EditText>(R.id.edit_text_login_password)!!

        val button: Button = view.findViewById(R.id.button_login)
        button.setOnClickListener {
            val usernameEmail: String = editTextUsernameEmail.text.toString()
            val password: String = editTextPassword.text.toString()

            if (usernameEmail.isBlank() || password.isBlank()) {
                Toast.makeText(view.context, "Invalid form", Toast.LENGTH_LONG).show()
                return@setOnClickListener
            }

            CoroutineScope(Dispatchers.IO).launch {
                try {
                    val response = RetrofitInstance.api.sendLoginRequest(LoginRequest(usernameEmail, password))

                    if (response.isSuccessful) {
                        if (response.body()!!.succeeded) {
                            MainActivity.instance.sharedPreferences.edit()
                                .putString(ApplicationConstants.authTokenPreferenceKey, "Bearer " + response.body()?.token)
                                .apply()

                            MainActivity.instance.notifyPreferencesChanged()
                        } else {
                            CoroutineScope(Dispatchers.Main).launch { Toast.makeText(view.context, "${response.body()!!.errors.first().code} - ${response.body()!!.errors.first().description}", Toast.LENGTH_LONG).show() }
                        }
                    } else {
                        CoroutineScope(Dispatchers.Main).launch { Toast.makeText(view.context, "Login up error", Toast.LENGTH_LONG).show() }
                    }
                } catch (e: Exception) {
                    CoroutineScope(Dispatchers.Main).launch { Toast.makeText(view.context, "Login up error: " + e.message, Toast.LENGTH_LONG).show() }
                }
            }
        }

        return view
    }
}