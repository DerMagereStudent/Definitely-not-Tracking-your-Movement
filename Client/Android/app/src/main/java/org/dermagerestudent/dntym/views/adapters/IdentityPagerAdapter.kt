package org.dermagerestudent.dntym.views.adapters

import androidx.fragment.app.Fragment
import androidx.fragment.app.FragmentManager
import androidx.lifecycle.Lifecycle
import androidx.viewpager2.adapter.FragmentStateAdapter
import org.dermagerestudent.dntym.views.LoginTabFragment
import org.dermagerestudent.dntym.views.SignUpTabFragment

class IdentityPagerAdapter(supportFragmentManager: FragmentManager, lifecycle: Lifecycle): FragmentStateAdapter(supportFragmentManager, lifecycle) {
    private val fragmentList = arrayListOf(
        LoginTabFragment(),
        SignUpTabFragment()
    )

    private val fragmentTitleList = arrayListOf(
        "Login",
        "Sign Up"
    )

    override fun getItemCount(): Int = this.fragmentList.size
    override fun createFragment(position: Int): Fragment = this.fragmentList[position]

    fun getTitle(position: Int) = this.fragmentTitleList[position]
}