using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
namespace Disney.DMOAnalytics.Framework {
	
public class DMOAnalyticsSysInfo {
//		private static string mIntBIServer = "https://int.api.disney.private/dismo/bi/v1";
	    private static string mIntBIServer = "http://blog.analytics.tapulous.com:8088";
	    private static string mBIServer = "https://api.disney.com/dismo/bi/v1";
		public static readonly string mVersion = "3.1.5";
		public static readonly string appStart = "app_start";
		public static readonly string appForeground = "app_foreground";
		public static readonly string appBackground = "app_background";
		public static readonly string appEnd = "app_end";
		public static readonly string gameAction = "game_action";
		public static readonly string moneyAction = "money";
		public static readonly string iOSMoneyAction = "payment_action";
		public static string CellophaneBIKey = "3962215E-C235-4FA8-BD8D-78BB9DA7D3B4";
		public static string CellophaneBISecret = "AC364A88C2D12A352DC81DC5FBBC9FEEE8BCCB3D9CA155FB";
		public static string CellophaneIntBIKey = "334E04DB-F88C-4FAA-8D82-0680EFC01364";
		public static string CellophaneIntBISecret = "2846EAA366F423DCA0E175B07D82770E918B8D27CD5B904E";
		public static bool isIntServerEnvironment = false;
		private static string mBundleIdentifier = "com.disney.HelloUnity";
		private static string mSessionHash;
		private static string mToken = null;
		private static string mAppVersion = "1.0";
		public static string getAppVersion() {
			return mAppVersion;
		}
		public static void setAppVersion( string appVersion) {
			mAppVersion = appVersion;
		}
		
		public static string getAnalyticsURL() {
			if (isIntServerEnvironment)
				return mIntBIServer;
			return mBIServer;
		}
		
		public static string getCellophaneKey() {
			if (isIntServerEnvironment)
				return CellophaneIntBIKey;
			return CellophaneBIKey;
		}
		
		public static string getCellophaneSecret() {
			if (isIntServerEnvironment)
				return CellophaneIntBISecret;
			return CellophaneBISecret;
		}
		
		public static string getAnalyticsKey() {
			return DMOAnalytics.SharedAnalytics.AnalyticsKey;
		}
		public static string getAnalyticsSecret() {
			return DMOAnalytics.SharedAnalytics.AnalyticsSecret;
		}
		
		public static void setBundelIdentifer (string bundleID) {
			mBundleIdentifier = bundleID;
		}
		
		public static string getBundleId() {
			return mBundleIdentifier;
		}
		
		public static string getModel() {
			return SystemInfo.deviceModel;
		}
		
		public static string getSystemVersion() {
			return SystemInfo.operatingSystem;
		}
		
		
		public static string getIsNewUser() {
			string isNewUser = "0";
			var data = PlayerPrefs.GetString("DMOIsNewUser");
			if(string.IsNullOrEmpty(data)) {
				isNewUser = "1";
				PlayerPrefs.SetString("DMOIsNewUser","0");
			}
			return isNewUser;
		}
		
		private static int kCCBlockSizeAES128 = 16;
		private static int kCCKeySizeAES128 = 16;
		private static string encryptString (string rawData) {
			if ( rawData == null) return null;
			string encryptedData  = "";
			byte[] rawEncodingData = System.Text.Encoding.UTF8.GetBytes(rawData);
			int memBlockSize = rawEncodingData.Length;
			if (memBlockSize % kCCBlockSizeAES128 > 0)
				memBlockSize = ((memBlockSize+kCCBlockSizeAES128) / kCCBlockSizeAES128) * kCCBlockSizeAES128;
			
			byte[] paddedBuffer = new byte[memBlockSize];
			Array.Copy(rawEncodingData, paddedBuffer, rawEncodingData.Length);
			int saltPlusJunkSize = 20;
			byte[] saltPlusJunk = {(byte)0x2d, /*junk */ (byte)0x37, (byte)0xa2, (byte)0xcd, /*junk */ (byte)0x28, (byte)0xd6, (byte)0x63, (byte)0x69, (byte)0xec, (byte)0x5b, (byte)0xb1, /*junk */ (byte)0xe9, (byte)0xae, (byte)0x1c, (byte)0x0a, (byte)0x5e, (byte)0x0c, (byte)0xc0, (byte)0xf3, /*junk */ (byte)0x5d};
			byte[] salt = new byte[kCCKeySizeAES128];
			int saltIndex=0;
			for (int junkIndex=0;junkIndex<saltPlusJunkSize;junkIndex++) {
				if (junkIndex!=1 && junkIndex!=4 && junkIndex!=11 && junkIndex!=19)
					salt[saltIndex++] = saltPlusJunk[junkIndex];
			}
			//start the encrypt
			RijndaelManaged cipher = new RijndaelManaged();
			cipher.Mode = CipherMode.ECB;
			cipher.Padding = PaddingMode.None;
			cipher.Key = salt;
			ICryptoTransform cTransform = cipher.CreateEncryptor();
			byte[] encrypted = cTransform.TransformFinalBlock(paddedBuffer, 0 , paddedBuffer.Length);
			if(encrypted != null && encrypted.Length > 0) {
				StringBuilder hexString = new StringBuilder();
				for (int i = 0; i < encrypted.Length; i++) {
					hexString.AppendFormat("{0:x2}", encrypted[i] & 0xff);
				}
				encryptedData = hexString.ToString();
			}
			return encryptedData;
		}
		
		/*public static string _getUniqueIdentifier() {
			string currentUniqueID = null;//SystemInfo.deviceUniqueIdentifier; <--- DON'T USE THIS OR UNITY WILL ADD READ_PHONE_STATE
			if (string.IsNullOrEmpty(currentUniqueID)) {
			var data = PlayerPrefs.GetString("DMOCurrentUniqueID");
			if(string.IsNullOrEmpty(data)) {
				currentUniqueID = System.Guid.NewGuid().ToString().ToUpper();
				PlayerPrefs.SetString("DMOCurrentUniqueID",currentUniqueID);
			} else {
				currentUniqueID = (string) data;
			}
			}
			return currentUniqueID;
		}*/
		
		
		//So for the  mac address, web player can't have access to macaddress, so we use system deviceUniqueIdentifier. It's the same across the browsers. 
		public static string getMToken() {
			if (string.IsNullOrEmpty(mToken)) {
				string uniqueStr = PulseEngine.Utils.GetDeviceGUID();
				mToken = encryptString( uniqueStr );
			}
			return mToken;
		}
		
		
		public static string getSessionId() {
			if (mSessionHash == null || mSessionHash.Length == 0) {
				mSessionHash = System.Guid.NewGuid().ToString().ToUpper();
			}
			return mSessionHash;
		}
		
		public static string getUTCTime() {
			return System.DateTime.UtcNow.ToString();
		}
		
		public static string getOnlineFlag() {
			if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
				return "1";
			if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
				return "2";
			return "0";
		}
		
		public static string getTimeStamp() {
			System.DateTime currentTime = System.DateTime.UtcNow;
	        System.DateTime uStartTime = System.Convert.ToDateTime("1/1/1970 0:00:00 AM");
	        System.TimeSpan timeStamp = currentTime.Subtract(uStartTime);
            long utcTime = ((((((timeStamp.Days * 24) + timeStamp.Hours) * 60) + timeStamp.Minutes) * 60) + timeStamp.Seconds);
			return utcTime.ToString();
		}
		
		public static Dictionary<string, object> GetSysInfo() {
			Dictionary<string, object> postData = new Dictionary<string, object>();
			postData.Add("DMOLibVersion", mVersion);
			postData.Add("api_key", getAnalyticsKey());
			postData.Add("app_version", getAppVersion()); 
			postData.Add("bundle_id", getBundleId());
			postData.Add("m_token", getMToken());
			postData.Add("model", getModel());
			postData.Add("onlineflag", getOnlineFlag());
			postData.Add("os_version", getSystemVersion());
			postData.Add("session_hash", getSessionId());
			postData.Add("timestamp", getTimeStamp());
			return postData;
		}
		
		public static Dictionary<string, object> GetCoreEventInfo() {
			Dictionary<string, object> postData = new Dictionary<string, object>();
			postData.Add("api_key", getAnalyticsKey());
			postData.Add("bundle_id", getBundleId());
			postData.Add("m_token", getMToken());
			postData.Add("onlineflag", getOnlineFlag());
			postData.Add("os_version", getSystemVersion());
			postData.Add("session_hash", getSessionId());
			postData.Add("timestamp", getTimeStamp());
			return postData;
		}
		
		
		public static Dictionary<string, object> GetTestDataDict() {
			Dictionary<string, object> queryKey = new Dictionary<string, object>();
			queryKey.Add("UTCTime","2013-08-21+15:47:26+-25200+(PDT)");
			queryKey.Add("api_key","2FD4ECB8-64B1-4610-BD3F-FF934AFA0FE1");
			queryKey.Add("bundle_id","com.disney.abtest");
			queryKey.Add("ios_advertising_id","2104d1f15a83ed00a95c123bea12d0c2b07a711e2b73c2f71ed50d8736583c0bbb7fb168f034b8456272afb06acb7e53");
			queryKey.Add("ios_vendor_id","77945b5e1e28f990244a5c0837a9d339ebf22f785e369a0e7b648e10326b2d38123b79642c997e270f6b9126b8f58855");
			queryKey.Add("is_new_user","0");
			queryKey.Add("m_token","5d6f6d21fe429b7d80bf88872e897000f9c4bc73dd294336c20b1f92c7e3e703");
			queryKey.Add("method","app_foreground");
			queryKey.Add("onlineflag","2");
			queryKey.Add("session_hash","791890AE-C8E8-4F15-95FC-314A929B8EDD");
			queryKey.Add("tag","clicked_link");
			queryKey.Add("timestamp","1377125247");
			return queryKey;
		}
}
}
