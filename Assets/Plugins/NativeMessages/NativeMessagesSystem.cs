using UnityEngine;
using System.Collections;

public class NativeMessagesSystem : MonoBehaviour
{
	protected static NativeMessagesSystem instance;
	
	public delegate void ButtonPressedEvent(int index);
	
	public static event ButtonPressedEvent OnButtonPressed;
	
	public static NativeMessagesSystem Instance {
		get {
			if (instance == null) {
				GameObject container = new GameObject("NativeMessagesSystem");
				instance = container.AddComponent<NativeMessagesSystem>();
				DontDestroyOnLoad(container);
			}
			
			return instance;
		}
	}
	
	void Awake()
	{
		instance = this;
		DontDestroyOnLoad(gameObject);
		
		#if !UNITY_EDITOR && UNITY_ANDROID
		Debug.Log("[CallStatic(INIT)] <<<<<<<<<<<<<<<<<");
		NativeMessageBinding.javaBinding.CallStatic("Init", name, "ButtonPressed");
		#endif
	}
	
	public void ShowMessage(string title, string message, string button1, string button2 = "", string button3 = "")
	{
		#if !UNITY_EDITOR && UNITY_IPHONE
		NativeMessageBinding.Native_ShowMessage(title, message, button1, button2, button3);
		#elif !UNITY_EDITOR && UNITY_ANDROID
		NativeMessageBinding.javaBinding.CallStatic("ShowMessage", title, message, button1, button2, button3);
		#else
		ButtonPressed("0");
		#endif
	}
	
	public void ButtonPressed(string buttonIndex)
	{
		Debug.Log("BUTTON PRESSED event received in Unity: " + buttonIndex);
		if (OnButtonPressed != null) 
		{
			int index = -1;
		
			if (!int.TryParse(buttonIndex, out index)) {
				index = -1;
			}
			Debug.Log("BUTTON PRESSED index: " + index);
			
			OnButtonPressed(index);
		}
	}
	
	public static void ScheduleNotification(string title, string message, long showTime)
	{
		Debug.Log("SCHEDULE NOTIFICATION: " + title + "  |  " + message + "  |  " + showTime);
		#if !UNITY_EDITOR && UNITY_IPHONE
		NativeMessageBinding.Native_ScheduleNotification(title, message, showTime);
		#elif !UNITY_EDITOR && UNITY_ANDROID
		NativeMessageBinding.javaBinding.CallStatic("ScheduleNotification", title, message, showTime);
		#endif
	}
	
	public static void CancelNotifications(string message)
	{
		Debug.Log("CANCEL NOTIFICATIONS: " + message);
		#if !UNITY_EDITOR && UNITY_IPHONE
		NativeMessageBinding.Native_CancelNotifications(message);
		#elif !UNITY_EDITOR && UNITY_ANDROID
		NativeMessageBinding.javaBinding.CallStatic("CancelNotification", message);
		#endif
	}
	
	public static string GetLocaleCountry()
	{
		#if !UNITY_EDITOR && UNITY_IPHONE
		return NativeMessageBinding.Native_GetLocaleCountry();
		#elif !UNITY_EDITOR && UNITY_ANDROID
		return NativeMessageBinding.javaBinding.CallStatic<string>("GetLocaleCountry");
		#else
		return "US";
		#endif
	}
}

