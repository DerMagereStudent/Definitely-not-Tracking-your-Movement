package org.dermagerestudent.dntym.views

import android.content.Context
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.EditText
import android.widget.Toast
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import org.dermagerestudent.dntym.R
import org.dermagerestudent.dntym.constants.ApplicationConstants
import org.dermagerestudent.dntym.networking.RetrofitInstance
import org.dermagerestudent.dntym.networking.contracts.requests.identity.LoginRequest
import org.dermagerestudent.dntym.networking.contracts.requests.identity.SignUpRequest

class SignUpTabFragment : Fragment() {
    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View? {
        // Inflate the layout for this fragment
        val view: View = inflater.inflate(R.layout.fragment_sign_up_tab, container, false)

        val editTextUsername = view.findViewById<EditText>(R.id.edit_text_signup_username)!!
        val editTextEmail = view.findViewById<EditText>(R.id.edit_text_signup_email)!!
        val editTextPassword = view.findViewById<EditText>(R.id.edit_text_signup_password)!!
        val editTextConfirmPassword = view.findViewById<EditText>(R.id.edit_text_signup_confirm_password)!!

        val button: Button = view.findViewById(R.id.button_signup)
        button.setOnClickListener {
            val username: String = editTextUsername.text.toString()
            val email: String = editTextEmail.text.toString()
            val password: String = editTextPassword.text.toString()
            val confirmedPassword: String = editTextConfirmPassword.text.toString()

            if (username.isBlank() || email.isBlank() || password.isBlank() || confirmedPassword.isBlank() || password != confirmedPassword) {
                Toast.makeText(view.context, "Invalid form", Toast.LENGTH_LONG).show()
                return@setOnClickListener
            }

            CoroutineScope(Dispatchers.IO).launch {
                try {
                    val response = RetrofitInstance.api.sendSignUpRequest(SignUpRequest(username, email, password))

                    if (response.isSuccessful) {
                        if (response.body()!!.succeeded)
                            CoroutineScope(Dispatchers.Main).launch { Toast.makeText(view.context, "Sign up", Toast.LENGTH_LONG).show() }
                        else
                            CoroutineScope(Dispatchers.Main).launch { Toast.makeText(view.context, "${response.body()!!.errors.first().code} - ${response.body()!!.errors.first().description}", Toast.LENGTH_LONG).show() }
                    } else {
                        CoroutineScope(Dispatchers.Main).launch { Toast.makeText(view.context, "Sign up error", Toast.LENGTH_LONG).show() }
                    }
                } catch (e: Exception) {
                    CoroutineScope(Dispatchers.Main).launch { Toast.makeText(view.context, "Sign up error: " + e.message, Toast.LENGTH_LONG).show() }
                }
            }
        }

        return view
    }
}