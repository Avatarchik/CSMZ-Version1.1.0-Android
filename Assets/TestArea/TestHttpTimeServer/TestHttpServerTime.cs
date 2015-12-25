using UnityEngine;
using System.Collections;

public class TestHttpServerTime : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () 
	{
		HttpServerTime.OnServerResponse += OnServerTimeResponse;
		
		HttpServerTime.Instance.InitServerList(TweaksSystem.Instance.stringValues["TimeServers"]);
		
		HttpServerTime.Instance.GetServerTime();
		
		yield return new WaitForSeconds(3f);
		
		HttpServerTime.Instance.GetServerTime();
		
		yield return new WaitForSeconds(1f);
		
		HttpServerTime.Instance.GetServerTime();
		
		yield return new WaitForSeconds(2f);
		
		HttpServerTime.Instance.GetServerTime();
		
		yield return new WaitForSeconds(3f);
		
		HttpServerTime.Instance.GetServerTime();
	}
	
	public void OnServerTimeResponse(bool receivedTime)
	{
		if (receivedTime)
		{
			Debug.Log("Successfully received time: " + HttpServerTime.LastReceivedServerTime);
			Debug.Log("Received time from server url: " + HttpServerTime.Instance.LastRespondedServerUrl);
			Debug.Log("Received time from server idx: " + HttpServerTime.LastRespondedServerIdx);
		}
		else 
		{
			Debug.Log("Failed to receive any server time!");
		}
	}
	
	void OnDestroy()
	{
		HttpServerTime.OnServerResponse -= OnServerTimeResponse;
	}
	
}
