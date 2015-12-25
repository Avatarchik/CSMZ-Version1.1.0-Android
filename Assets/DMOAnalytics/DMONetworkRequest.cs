
using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
namespace Disney.DMOAnalytics.Framework {
	
		public class DMONetworkRequest {
		
		//TODO: timeout request
		private const int mTimeOut = 60 * 1000;
		
		private MonoBehaviour mGameObj;
		public delegate void NetworkRequestCallback ( DMONetworkRequest request, bool isSuccess );	
		public event NetworkRequestCallback requestCallback;
		
		
		
		//Singleton 
		private static DMONetworkRequest mInstance;
		
		
		private DMONetworkRequest () {
			_eventQueue = new Queue<Dictionary<string, object>>();
		}
		
		public static DMONetworkRequest Instance {
			get{
				if (mInstance == null) {
					mInstance = new DMONetworkRequest();
				}
				return mInstance;
			}
		}
		
		//queue of the log events
		private Queue<Dictionary<string, object> > _eventQueue;
		
		//for custom event
		public void logEvent(MonoBehaviour GameObj, string EventInfo) {
			mGameObj = GameObj;
			Dictionary<string, object> postData = DMOAnalyticsSysInfo.GetCoreEventInfo();
			postData.Add( "method" , EventInfo );
			StartCoroutine( postData );
		}
		
		public void logEventWithContext(MonoBehaviour GameObj, string EventInfo, Dictionary<string, object> DataDetails) {
			mGameObj = GameObj;
			string dataStr = DMOAnalyticsHelper.GetStringFromDictionary(DataDetails);
			Dictionary<string, object> postData = DMOAnalyticsSysInfo.GetCoreEventInfo();
			postData.Add( "method" , EventInfo );
			postData.Add( "data", dataStr);
			StartCoroutine( postData );
		}
		
		//for app start
		public void logAppStartEvent(MonoBehaviour GameObj) {
			mGameObj = GameObj;
			Dictionary<string, object> postData = DMOAnalyticsSysInfo.GetSysInfo();
			postData.Add( "method" , "app_start" );
			postData.Add( "is_new_user" , DMOAnalyticsSysInfo.getIsNewUser());
			postData.Add( "tag" , "clicked_link" );
			StartCoroutine( postData );
			//for debugging only 
			//StartCoroutine( DMOAnalyticsSysInfo.GetTestDataDict() );
		}
		//for app foreground, background
		public void logAppEvent(MonoBehaviour GameObj, string EventInfo) {
			mGameObj = GameObj;
			Dictionary<string, object> postData = DMOAnalyticsSysInfo.GetCoreEventInfo();
			postData.Add( "method" , EventInfo );
			if ( EventInfo.Equals(DMOAnalyticsSysInfo.appForeground) ) {
				postData.Add( "is_new_user" , DMOAnalyticsSysInfo.getIsNewUser() );
				postData.Add( "tag" , "clicked_link" );
			}
			StartCoroutine( postData );
		}
		
		public void logGameEvent(MonoBehaviour GameObj, Dictionary<string, object> GameData) {
			mGameObj = GameObj;
			string gameDataStr = DMOAnalyticsHelper.GetStringFromDictionary(GameData);
			Dictionary<string, object> postData = DMOAnalyticsSysInfo.GetCoreEventInfo();
			postData.Add( "data" , gameDataStr );
			postData.Add( "method", DMOAnalyticsSysInfo.gameAction);
			postData.Add( "tag", DMOAnalyticsSysInfo.gameAction);
			StartCoroutine( postData );
		}
		
		public void logMoneyEvent(MonoBehaviour GameObj, Dictionary<string, object> MoneyData) {
			mGameObj = GameObj;
			string moneyDataStr = DMOAnalyticsHelper.GetStringFromDictionary(MoneyData);
			Dictionary<string, object> postData = DMOAnalyticsSysInfo.GetCoreEventInfo();
			postData.Add( "data" , moneyDataStr );
			postData.Add( "method", DMOAnalyticsSysInfo.moneyAction);
			postData.Add( "tag", DMOAnalyticsSysInfo.moneyAction);
			postData.Add( "source", "CCBILL");
			postData.Add( "subtype", "CCBILL");
			StartCoroutine( postData );
		}
		
		private static String multipartSeparator = "GOGO-87dae883c6039b244c0d341f45f8-SEP";
		
		private string getTimeStamp() {
			
			System.DateTime currentTime = System.DateTime.UtcNow;
	        System.DateTime uStartTime = System.Convert.ToDateTime("1/1/1970 0:00:00 AM");
	        System.TimeSpan timeStamp = currentTime.Subtract(uStartTime);
            long utcTime = ((((((timeStamp.Days * 24) + timeStamp.Hours) * 60) + timeStamp.Minutes) * 60) + timeStamp.Seconds);
			return utcTime.ToString();
		}
		
		private string getHostURL() {
			return DMOAnalyticsSysInfo.getAnalyticsURL();
		}
		
		//compare the two dictionary for data queueing 
		// if no data field, only make sure timestamp and method are the same, that will be coreevents, app_start, etc
		// if there is data, then data needs to be same too, that will be gameaction, etc
		private bool _compareTwoDictionary ( Dictionary<string, object> GameData, Dictionary<string, object> CollectData) {
			bool sameOld = false;
			var imethod = GameData["method"];
			var itimestamp = GameData["timestamp"];
			bool isdata = GameData.ContainsKey( "data" );
			if ( imethod.Equals( CollectData["method"] ) && itimestamp.Equals( CollectData["timestamp"] )) {
				if (!isdata) {
					sameOld = true;
				} else {
					var idata = GameData["data"];
					if (idata.Equals ( CollectData["data"] )) {
						sameOld = true;
					}
				}
			}
			
			return sameOld;
		}
		
		private void _addGameDataToEventQueue ( Dictionary<string, object> GameData ) {
			bool shouldAdd = true;
			foreach(Dictionary<string, object> CollectData in _eventQueue) {
				if ( _compareTwoDictionary(GameData, CollectData) ) {
					shouldAdd = false;
					DMOAnalyticsHelper.Log("GameData is duplicate, no need to add again: "+ GameData["method"]);
					break;
				}
			}
			
			if (shouldAdd)
				_eventQueue.Enqueue(GameData);
		}
		
		public void FlushQueue() {
			if (! DMOAnalyticsHelper.ICanUseNetwork) return;
			if(_eventQueue.Count <= 0 ) return;
			DMOAnalyticsHelper.Log("Flush the analytics queue");
			while (_eventQueue.Count > 0) {
				Dictionary<string, object> postData = _eventQueue.Peek();
				StartCoroutine( postData );
				_eventQueue.Dequeue();
			}
		}
		
		private void StartCoroutine(Dictionary<string, object> GameData) {
			if (mGameObj == null) {
				//handle the exceptions when the gameobject is null
				var iGameOjb = new GameObject("DMOAnalytics Coroutines");
				UnityEngine.Object.DontDestroyOnLoad(iGameOjb);
				mGameObj = iGameOjb.AddComponent<MonoBehaviour>();
			}
			
			if ( GameData == null ) return;
			
			//make sure there is internet access
			bool _canPost = Application.internetReachability != NetworkReachability.NotReachable && DMOAnalyticsHelper.ICanUseNetwork;
			if (_canPost) {
				mGameObj.StartCoroutine(_runEventRequest( GameData ));
			} else {
				//if the data is not in the queue, added it to the queue
				_addGameDataToEventQueue(GameData);
			}
		}
		
		private IEnumerator _runEventRequest(Dictionary<string, object> postData) {
			
		Dictionary<string, string> headers = new Dictionary<string, string>();
		string authorizationField = String.Format("FD {0}:{1}", DMOAnalyticsSysInfo.getCellophaneKey(), DMOAnalyticsSysInfo.getCellophaneSecret());
		headers.Add( "Authorization", authorizationField );
			
			
	    string contentType = "multipart/form-data; boundary=" + multipartSeparator;
		headers.Add( "Content-Type", contentType);
		
			
		//create the hash for the sig
		string apiSer = _signatureFromPostData(postData);
		postData.Add( "api_sig", apiSer);
		//format the postdata
		string dataStr = getDataBodyStr(postData);
		
		string hostURL = getHostURL();
		DMOAnalyticsHelper.Log("Connecting to Server: "+ hostURL);
		WWW www = new WWW( hostURL, System.Text.Encoding.UTF8.GetBytes(dataStr), headers);
		
		yield return www;
		
		bool isPosted;
		isPosted = ( www.isDone == true && www.error == null && www.text != null );
			
			
			if(!isPosted) {
				DMOAnalyticsHelper.Log("Errr in WWW request:"+www.error);
			}

			if(isPosted) {
				try {
					var contentLen = www.responseHeaders["CONTENT-LENGTH"];
					int textLen = Convert.ToInt32(contentLen);
					if ( textLen < 1) {
						isPosted = false;
					} 
				} catch (Exception ex) {
					isPosted = false;
					DMOAnalyticsHelper.Log("Errr in WWW response: "+ex.ToString());
				}	
			}
			
		DMOAnalyticsHelper.Log("WWW request is posted");
		if (requestCallback != null)	
				requestCallback(this, isPosted);
		
		}
		
		private string getDataBodyStr(Dictionary<string, object> postData) {
		//format the postdata
		string dataStr = "";
		
		foreach(KeyValuePair<string, object> item in postData) {
				object iValue = item.Value;  
				if(iValue != null)
				dataStr += String.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n",multipartSeparator, item.Key, (string)iValue);
		}
		//end of the post
			dataStr+="--";
			dataStr+=multipartSeparator;
			dataStr+="--\r\n";
			return dataStr;
		}
		
		private string _signatureFromPostData( Dictionary<string, object> postData) {
			//get all the keys
			string[] postPairs = new string[postData.Keys.Count];
			string[] postKeys = new string[postData.Keys.Count];
			postData.Keys.CopyTo(postKeys, 0);
			Array.Sort(postKeys, StringComparer.Ordinal); //sort bydefault using compareTo, use Ordinal will put uppercase letters first.
			int i = 0;
			foreach (string postKey in postKeys) {
				string postValue = (string)postData[postKey];
				string iValue = postValue.Replace(" ", "+");
				postPairs[i] = postKey + "=" + System.Uri.EscapeDataString(iValue);
				i++;
			}
			
			string hash =  System.Uri.EscapeDataString(String.Join("&",postPairs)); 
			 
			string rawHash =  _tempFixForDoubleEncoding(hash);
			
			//DMOAnalyticsHelper.Log("rawHash: "+ rawHash);
			string keyStr = String.Format("{0}&", DMOAnalyticsSysInfo.getAnalyticsSecret());
			byte[] secretData = System.Text.Encoding.UTF8.GetBytes(keyStr);
			byte[] clearTextData = System.Text.Encoding.UTF8.GetBytes(rawHash);
			
			HMACSHA1 hmac = new HMACSHA1(secretData);
			hmac.Initialize();
			byte[] result = hmac.ComputeHash(clearTextData);
			string base64EncodeResult = Convert.ToBase64String(result);
			//DMOAnalyticsHelper.Log("encoded result: "+ base64EncodeResult);
			return base64EncodeResult;
		}
		
		private string _tempFixForDoubleEncoding (string inString) {
			string outStr = inString.Replace("(", "%2528");
			outStr = outStr.Replace(")","%2529");
			return outStr;
		}
		
    }
	
}