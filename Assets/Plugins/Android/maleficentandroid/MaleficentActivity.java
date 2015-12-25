package com.disney.maleficent;

import com.comscore.analytics.comScore;
import com.unity3d.player.UnityPlayer;
import android.os.Bundle;
import android.util.Log;
import android.provider.Settings;
import android.content.res.Configuration;
import android.content.pm.ActivityInfo;

public class MaleficentActivity {
	public static void onCreate(Bundle savedInstanceState) {
		Log.d("OverrideActivity", "onCreate called!");
		comScore.setAppContext(UnityPlayer.currentActivity.getApplicationContext());
		comScore.setAppName("Maleficent Free Fall");
		comScore.setCustomerC2("6035140");
		comScore.setPublisherSecret("bacd860dcd22dd180bdcb7c680f64060");
	}
	
	public static void onPause() {
		Log.d("OverrideActivity", "onPause called!");
		comScore.onExitForeground();
	}
	
	public static void onResume() {
		Log.d("OverrideActivity", "onResume called!");
		comScore.onEnterForeground();
		
		//Detect if 'lock Auto-Rotation' option in android is enabled / disabled (0 - locked, 1 - locked)
		int accelRotation = android.provider.Settings.System.getInt(UnityPlayer.currentActivity.getContentResolver(), Settings.System.ACCELEROMETER_ROTATION, 0);
		
		//Send lock Auto-Rotation status to Unity, to block / unblock rotation accordingly.
		UnityPlayer.UnitySendMessage("OrientationListener", "HandleLockRotation", Integer.toString(accelRotation));
	}
	
	/*public static void onConfigurationChanged(Configuration newConfig) {
		int orientation = newConfig.orientation;
		Log.d("OverrideActivity", "onConfigurationChanged");
		
		if(orientation == Configuration.ORIENTATION_PORTRAIT) //Lock rotation on portrait
			UnityPlayer.UnitySendMessage("OrientationListener","HandleLockRotation", Integer.toString(0));
		else //Unlock rotation in landscape
			UnityPlayer.UnitySendMessage("OrientationListener","HandleLockRotation", Integer.toString(1));
			
		UnityPlayer.currentActivity.setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_SENSOR);
	}*/
} 