package org.dermagerestudent.dntym.views

import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.viewpager2.widget.ViewPager2
import com.google.android.material.tabs.TabLayout
import com.google.android.material.tabs.TabLayoutMediator
import org.dermagerestudent.dntym.R
import org.dermagerestudent.dntym.views.adapters.IdentityPagerAdapter

class IdentityFragment : Fragment() {
    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View? {
        val view: View = inflater.inflate(R.layout.fragment_identity, container, false)!!

        val tabLayout: TabLayout = view.findViewById(R.id.tab_layout_identity)!!
        val viewPager: ViewPager2 = view.findViewById(R.id.view_pager_identity)!!
        val adapter = IdentityPagerAdapter(this.requireActivity().supportFragmentManager, this.requireActivity().lifecycle)

        viewPager.adapter = adapter
        tabLayout.tabGravity = TabLayout.GRAVITY_FILL

        TabLayoutMediator(tabLayout, viewPager) {
            tab, position -> tab.text = adapter.getTitle(position)
        }.attach()

        return view
    }
}