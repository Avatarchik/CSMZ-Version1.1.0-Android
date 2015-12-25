using UnityEngine;
using System.Collections;
using System;

public class AMPListenerEventArgs : EventArgs
{
	public bool Result {get; set; }
	public string Message {get; set;}
	public string Error {get; set;}
	public string FilenameDownloaded {get; set;}
}

public class AMPSListener : MonoBehaviour {
	
	public delegate void AMPSListenerDelegate (object sender, AMPListenerEventArgs e);
	
	public event AMPSListenerDelegate AMPSManagerInit;
	private void RaiseAMPSManagerInit(AMPListenerEventArgs e)
	{	
		if (AMPSManagerInit != null)
			AMPSManagerInit(this, e);
	}
	
	public event AMPSListenerDelegate FileDownloaded;
	private void RaiseFileDownloaded(AMPListenerEventArgs e)
	{	
		if (FileDownloaded != null)
			FileDownloaded(this, e);
	}
	
	public event AMPSListenerDelegate FileDownloadStarted;
	private void RaiseFileDownloadStarted(AMPListenerEventArgs e)
	{	
		if (FileDownloadStarted != null)
			FileDownloadStarted(this, e);
	}
	
	
//	protected static AMPSListener instance;
	protected static bool dontReload = false;
	
	
	/// <summary>
	/// Creates a new AMPS listener instance with the specified game object name. Each AMPS AssetManager will have it's own listener game object. 
	/// Note: The GameObject names must NOT be the same.
	/// </summary>
	/// <returns>
	/// The instance.
	/// </returns>
	/// <param name='_name'>
	/// _name.
	/// </param>
	public static AMPSListener CreateInstance(string _name)
	{
		GameObject container = GameObject.Instantiate(Resources.Load("AMPSListener") as GameObject) as GameObject;
		container.name = _name;
		DontDestroyOnLoad(container);
		
		return container.GetComponent<AMPSListener>();
	}
	
	void Awake ()
	{
		DontDestroyOnLoad(gameObject);
	}
	
//	public void Init()
//	{
//		Debug.Log("AMPListener: Init ");
//	}
	
	#region Listeners from AMPSBinding
	
	public void ErrorInitAMPManager(string messageError)
	{
		Debug.Log("AMPListener: " + messageError);
		
		AMPListenerEventArgs args = new AMPListenerEventArgs();
		args.Error = messageError;
		args.Result = false;
		args.Message = "AMPSManager was not initialized";
		
		this.RaiseAMPSManagerInit(args);
	}
	
	public void AMPManagerDidInit(string successMessage)
	{
		Debug.Log("AMPListener: " + successMessage);
		
		AMPListenerEventArgs args = new AMPListenerEventArgs();
		args.Error = string.Empty;
		args.Result = true;
		args.Message = "AMPSManager was initialized";
		
		this.RaiseAMPSManagerInit(args);
		
	}
	
	public void ErrorDownloadingAsset(string _assetName)
	{
		Debug.Log("AMPListener download error for asset: " + _assetName);
		
		AMPListenerEventArgs args = new AMPListenerEventArgs();
		args.Error = "Error downloading asset: " + _assetName;
		args.Result = false;
		args.Message = "Error downloading asset from AMPS: " + _assetName;
		args.FilenameDownloaded = _assetName;
		
		RaiseFileDownloaded(args);
	}
	
	public void DownloadFileStarted(string fileName)
	{
		Debug.Log("AMPListener: " + fileName);
		
		AMPListenerEventArgs args = new AMPListenerEventArgs();
		args.Error = string.Empty;
		args.Result = true;
		args.Message = "Starting download " + fileName;
		args.FilenameDownloaded = fileName;
		
		RaiseFileDownloadStarted(args);
	}
	
	public void DownloadFileDidFail(string fileName)
	{
		Debug.Log("AMPListener: " + fileName);
		
		AMPListenerEventArgs args = new AMPListenerEventArgs();
		args.Error = "File did fail downloaded";
		args.Result = false;
		args.Message = "ErrorDownloading asset from AMPS";
		args.FilenameDownloaded = fileName;
		
		RaiseFileDownloaded(args);
	}
	
	public void DownloadFileDidFinish(string fileName)
	{
		Debug.Log("AMPListener: " + fileName);
		
		AMPListenerEventArgs args = new AMPListenerEventArgs();
		args.Error = string.Empty;
		args.Result = true;
		args.Message = "Asset downloaded correcty";
		args.FilenameDownloaded = fileName;
		
		RaiseFileDownloaded(args);
	}
	
	
	
	#endregion
	
	void OnDestroy()
	{
		dontReload = true;
	}

}
