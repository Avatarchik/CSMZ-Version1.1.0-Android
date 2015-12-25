using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class HttpServerTime : MonoBehaviour 
{
	public delegate void OnServerResponseDelegate(bool receivedTime);
		
	/// <summary>
	/// The server check delay (in seconds). If you request the server again within this delay,
	/// the last received server time will be returned and the request won't be re-done.
	/// </summary>
	public static float serverCheckDelay = 5f;
	private static float lastServerCheckTime = -999999f;
		
	private static HttpServerTime instance;
	
	protected static System.DateTime? lastServerTime = null;
	protected static int lastServerIdx = -1;
	
	public static event OnServerResponseDelegate OnServerResponse;
	
	protected WWW httpRequest = null;
	
	// A list of servers urls that will be used in order to get the time by doing an HTTP GET request and reading their response header
	public List<string> timeServers;

	
	public static HttpServerTime Instance 
	{
		get
		{
			if (instance == null)
			{
				new GameObject("ServerTime", typeof(HttpServerTime));
			}
			
			return instance;
		}
	}
	
	void Awake()
	{
		// Check if there's alreay another instance of this singleton.
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}
		
		instance = this;

		timeServers = new List<string>(4);
	}
	
	public static System.DateTime? LastReceivedServerTime
	{
		get
		{
			return lastServerTime;
		}
		
		protected set 
		{
			lastServerTime = value;
		}
	}
		
	public string LastRespondedServerUrl
	{
		get
		{
			if (lastServerIdx >= 0)
			{
				return timeServers[lastServerIdx];
			}
		
			return "";
		}
	}
	
	public static int LastRespondedServerIdx
	{
		get
		{
			return lastServerIdx;
		}
		
		protected set
		{
			lastServerIdx = value;
		}
	}
	
	/// <summary>
	/// Inits the server list from a string of server urls separated by the '|' character.
	/// </summary>
	/// <param name='serverUrls'>
	/// Server urls.
	/// </param>
	public void InitServerList(string serverUrls)
	{
		string[] urls = serverUrls.Split('|');

		timeServers.Clear();
		
		// Populate the servers list
		for (int i = 0; i < urls.Length; i++)
		{
			timeServers.Add( urls[i].Trim() );
			Debug.Log("Registering time server: " + timeServers[i]);
		}
	}
			
	public void GetServerTime(OnServerResponseDelegate onServerResponseAction = null)
	{
		// Return the last result if there's a request already pending or if the request is within the time delay limit.
		if (httpRequest != null || Time.realtimeSinceStartup - lastServerCheckTime < serverCheckDelay)
		{
			if (httpRequest == null) 
			{
				Debug.LogWarning("[HttpServerTime] Querying within the delay limit: " + serverCheckDelay + " seconds!");
				RaiseOnServerResponseEvent(LastReceivedServerTime != null, onServerResponseAction);
			}
		}
		else 
		{
			StartCoroutine(GetServerTimeCoroutine(onServerResponseAction));
		}
	}
	
	protected IEnumerator GetServerTimeCoroutine(OnServerResponseDelegate onServerResponseAction = null)
	{
		bool requestSuccessfull = false;

		for (int i = 0; i < timeServers.Count; i++)
		{
			httpRequest = new WWW(timeServers[i]);
			
			yield return httpRequest;
			
			// Server request failed. Try the next server.
			if (httpRequest.error != null)
			{
				Debug.Log("[HttpServerTime] Failed connection to server: " + timeServers[i] + " error: " + httpRequest.error);
				continue;
			}
			else
			{
				Dictionary<string, string> responseHeaders = httpRequest.responseHeaders;
				DebugDictionary("SERVER TIME", responseHeaders);

				requestSuccessfull = true;
				
				string serverTime;				
				if ( responseHeaders.TryGetValue("DATE", out serverTime) )
				{
					System.DateTime dateTime;
					if ( System.DateTime.TryParse(serverTime, out dateTime) )
					{
						// We've parsed a server time. Populate response fields and stop here.
						requestSuccessfull = true;
						LastReceivedServerTime = dateTime;
						LastRespondedServerIdx = i;
						
						break;
					}
					else
					{
						Debug.LogWarning("[HttpServerTime] Couldn't parse date time from server: " + serverTime);
						continue;
					}
				}
				else
				{
					Debug.LogWarning("[HttpServerTime] Couldn't find DATE key in the response headers from server: " + timeServers[i]);
					continue;
				}
			}
		}
		
		// Update the last server check time if the request was successfull.
		if (requestSuccessfull) 
		{
			lastServerCheckTime = Time.realtimeSinceStartup;
		}
		
		// Notify request response.
		RaiseOnServerResponseEvent(requestSuccessfull, onServerResponseAction);
		
		// Finished
		httpRequest = null;
	}
	
	protected void RaiseOnServerResponseEvent(bool requestSuccessfull, OnServerResponseDelegate onServerResponseAction = null)
	{
		if (OnServerResponse != null)
		{
			OnServerResponse(requestSuccessfull);
		}
		
		if (onServerResponseAction != null) 
		{
			onServerResponseAction(requestSuccessfull);
		}
	}
		
	public static void DebugDictionary(string debugMsg, Dictionary<string, string> dict)
	{
		StringBuilder strBuilder = new StringBuilder();
		foreach(string key in dict.Keys)
		{
			strBuilder.AppendLine(string.Format("{0}: {1}", key, dict[key]));
		}
		
		Debug.Log(strBuilder.ToString());
	}
	
	void OnDestroy()	
	{
		if (httpRequest != null && !httpRequest.isDone)
		{
			try
			{
				httpRequest.Dispose();
			}
			catch(System.Exception ex)
			{
				Debug.LogWarning("[HttpServerTime] Error while trying to force stop in progress WWW request: " + ex.Message);
			}
		}
	}
}

