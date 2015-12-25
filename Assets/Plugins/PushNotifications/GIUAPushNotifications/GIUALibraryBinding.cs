using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class GIUALibraryBinding : MonoBehaviour 
{
	#if UNITY_IPHONE
	[DllImport("__Internal")]
    private static extern void _requestAppRegistration();
	#endif
	
    public static void requestAppRegistration()
    {
		#if UNITY_IPHONE
		
    	_requestAppRegistration();
		
		#endif
    }
	
}