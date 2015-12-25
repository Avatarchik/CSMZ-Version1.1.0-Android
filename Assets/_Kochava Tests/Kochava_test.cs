using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;

public class Kochava_test : MonoBehaviour {
	
	IEnumerator  Start ()
	{
		/*Hashtable eventData = new Hashtable () {
			{ "action", "event" },
			{ "data", new Hashtable () {
				{ "event_data", "123" },
				{ "event_name", "send track event" },
			}},
			{ "kochava_app_id", "koconversionsdemo174ea19bc63928c" },
			{ "kochava_device_id", "KVcd9d8b49a3a1152d1e14613f9aa12b2c" },
		};
		
		string postData = JsonWriter.Serialize (eventData);*/
		
		
		string postData = "";
		
		postData = "{\"kochava_app_id\":\"kosimsfreeplayiosea4385112e0941dfa3\",\"ip_address\":\"10.253.120.128\",\"device_id\":{\"mac\":\"7C:6D:62:DF:70:80\",\"idfv\":\"123123123\",\"idfa\":\"23424324\",\"udid\":\"88a3b9b5673d6c9cbe64e20865dc5608819f87f9\"},\"data\":[{\"event_name\":\"Resume\",\"usertime\":\"2012-06-12 01:30:35 +0000\",\"geo_lat\":\"\",\"geo_long\":\"\"}]}";
		Debug.Log(postData);
		WWW www = new WWW ("https://control.kochava.com/track/kvTracker.php", System.Text.Encoding.UTF8.GetBytes (postData), new Dictionary <string, string>() {{ "Content-Type", "application/xml" }});
		yield return www;
		Debug.Log(www.text);
		
		postData = "{\"sdk_protocol\":\"2\",\"action\":\"session\",\"kochava_device_id\":\"kodavidiostest496513e08ee27a8c\",\"data\":{\"state\":\"launch\",\"uptime\":\"0.00\",\"usertime\":\"1362278808.82\"},\"sdk_version\":\"Unity3D-20130128\",\"debug\":\"true\",\"kochava_app_id\":\"kodavidiostest496513e08ee27a8c\"}";
		Debug.Log(postData);
		www = new WWW ("https://control.kochava.com/track/kvinit", System.Text.Encoding.UTF8.GetBytes (postData), new Dictionary <string, string>() {{ "Content-Type", "application/xml" }});
		yield return www;
		Debug.Log(www.text);
		
	}
	
}

/*
"usertime":"1358985884",
      "uptime":"112",
      "event_data":"dummyData",
      "geo_lon":"0",
      "updelta":"110",
      "geo_lat":"0",
      "event_name":"initialTester"


action = event;
    data =     {
        "event_data" = "";
        "event_name" = "send track event";
    };
    "kochava_app_id" = koconversionsdemo174ea19bc63928c;
    "kochava_device_id" = KVcd9d8b49a3a1152d1e14613f9aa12b2c;
    
   */