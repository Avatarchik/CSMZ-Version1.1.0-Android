using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Burstly global events listener that listens for all native Burstly events from all banners and interstitials and 
/// forwards the events related to an Ad banner id to one or more GameObjects.
/// 
/// Note: this Burstly global events listener is required because the current version of Burstly Unity wrapper
/// doesn't registering multiple GameObjects to receive targeted events for only for certain registered Ad.
/// There can't be multiple listeners like this in the same scene!
/// 
/// The events raised on the registered gameobjects are in the format:
/// OnXXX(string bannerId) where XXX is any name from <see cref="BurstlyEvent"/>  enum defined in this class.
/// </summary>
public class BurstlyEventsListener : UnitySingleton<BurstlyEventsListener> 
{
	/// <summary>
	/// Burstly event type as described in the native implementation.
	/// </summary>
	public enum BurstlyEvent : int
	{
	    BurstlyEventSucceeded           = 0x01,
	    BurstlyEventFailed              = 0x02,
	    BurstlyEventTakeoverFullscreen  = 0x04,
	    BurstlyEventDismissFullscreen   = 0x08,
	    BurstlyEventHidden              = 0x10,
	    BurstlyEventShown               = 0x20,
	    BurstlyEventCached              = 0x40,
	    BurstlyEventClicked             = 0x80
	}
		
	public bool dontDestroy = false;
	
	/// <summary>
	/// The registered  ads that will receive events from this listener.
	/// </summary>
	[System.NonSerialized]
	public Dictionary<string, List<GameObject>> registeredBurstlyAds = new Dictionary<string, List<GameObject>>();
	
	
	protected override void Awake ()
	{
		base.Awake ();
	
		if (hasDuplicateInstances) {
			return;
		}
		
		if (dontDestroy) {
			DontDestroyOnLoad(gameObject);
		}
		
		// Set this GameObject as the global events listener for Burstly.
		BurstlyAds.setCallbackGameObjectName(name);		
	}

	/// <summary>
	/// Register an Ad to receive only events related to him.
	/// </summary>
	/// <param name='bannerId'>
	/// Banner identifier.
	/// </param>
	/// <param name='eventsListener'>
	/// Events listener gameobject that will receive events related to the specified banner.
	/// </param>
	public void RegisterEventsListener(string bannerId, GameObject eventsListener)
	{
		List<GameObject> listenersList;
		
		if ( !registeredBurstlyAds.TryGetValue(bannerId, out listenersList) )
		{
			listenersList = new List<GameObject>();
			registeredBurstlyAds.Add(bannerId, listenersList);
		}

		if ( !listenersList.Contains(eventsListener) ) {
			listenersList.Add(eventsListener);
		}
	}
	
	public void UnregisterEventsListener(string bannerId, GameObject eventsListener)
	{
		List<GameObject> listenersList;
		
		if ( registeredBurstlyAds.TryGetValue(bannerId, out listenersList) ) {
			listenersList.Remove(eventsListener);
		}
	}
	
	/// <summary>
	/// The method called from the native Burstly implementation for any Burstly event for banners or interstitials.
	/// The "eventInfo" is a pipe delimited string containing  the banner id (placement name) and an integer describing the BurstlyEvent enum.
	/// </summary>
	/// <param name='eventInfo'>
	/// Event info.
	/// </param>
	private void BurstlyCallback(string eventInfo)
	{
		string[] eventData = eventInfo.Split('|');
		
		// Ignore other event info format than the one we expect (for safety).
		if (eventData.Length != 2) {
			return;
		}
		
		string bannerId = eventData[0].Trim();
		int eventType;
		
		if ( !int.TryParse(eventData[1], out eventType) ) {
			// Ignore this event if we can't convert this to int.
			return;
		}
		
		HandleNativeEvent(bannerId, (BurstlyEvent)eventType);
	}
	
	/// <summary>
	/// Handles the native event and forwards it to all GameObjects registered to this event.
	/// </summary>
	/// <param name='bannerId'>
	/// Banner identifier.
	/// </param>
	/// <param name='eventType'>
	/// Event type.
	/// </param>
	private void HandleNativeEvent(string bannerId, BurstlyEvent eventType)
	{
		List<GameObject> listenersList;
		
		if ( registeredBurstlyAds.TryGetValue(bannerId, out listenersList) )
		{
			for (int i = 0; i < listenersList.Count; i++)
			{
				listenersList[i].SendMessage(string.Format("On{0}", eventType.ToString()), bannerId, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	protected override void OnDestroy()
	{
		base.OnDestroy();
		
		foreach(List<GameObject> listenersList in registeredBurstlyAds.Values)
		{
			listenersList.Clear();
			listenersList.TrimExcess();
		}

		registeredBurstlyAds.Clear();
	}
}
