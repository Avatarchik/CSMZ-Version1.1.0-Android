using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Linq;
using System.IO.Compression;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using System.Text;

public class LocalizationServerManagerDelegateEventArgs : EventArgs
{
	public bool Result {get; set;}
	public bool Downloaded {get; set;}
	public string Message {get; set;}
	public string Error {get; set;}
}

public class LocalizationServerManager : MonoBehaviour {
	
	public const string FILE_NAME = "localization.zip";
	public const string FILE_VERSION_KEY = "localization_version";
	private const string LAST_UPDATE = "LastLocalizationSyncrhonization";
	
	public int Cache_Time_For_Update = 0; // In hours
	
	private string DataPath = "";
	
	public AMPSListener eventsListener;

	#region Singleton
	
	protected static LocalizationServerManager instance;
	
	
	void Awake ()
	{
		instance = this;
		DontDestroyOnLoad(gameObject);
		#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
		DataPath = Application.persistentDataPath + "/localization/";
		#elif UNITY_EDITOR
		DataPath = "StoredData/localization/";
		#endif
		
		eventsListener = AMPSListener.CreateInstance("LocalizationAmpsListener");
	}
	
	public static LocalizationServerManager Instance 
	{
		get {
			if (instance == null ) 
			{
				GameObject container = GameObject.Instantiate(Resources.Load("LocalizationServerManager") as GameObject) as GameObject;
				DontDestroyOnLoad(container);
			}
			
			return instance;
		}
	}
	
	#endregion
	
	#region Delegates
	
	/// <summary>
	/// User delegate.
	/// </summary>
	public delegate void LocalizationServerDelegate(object sender, LocalizationServerManagerDelegateEventArgs e);
	public event LocalizationServerDelegate DownloadCompleted;
	
	private void RaiseLocalizationServerDelegate(LocalizationServerManagerDelegateEventArgs e)
	{
		if (DownloadCompleted != null)
			DownloadCompleted(this, e);
	}
	
	
	void RaiseLocalizationCallback (bool success, bool downloaded, string message, string error, System.Action<LocalizationServerManagerDelegateEventArgs> onResult)
	{
		LocalizationServerManagerDelegateEventArgs args = new LocalizationServerManagerDelegateEventArgs(){
			Result = success,
			Downloaded = downloaded,
			Message = message,
			Error = error
		};
		
		if (onResult != null)
			onResult(args);
		
		RaiseLocalizationServerDelegate(args);
	}
	
	
	#endregion

	public bool HasLanguageFile(string lang, string sheetTitle)
	{
	#if UNITY_WEBPLAYER
		Debug.LogWarning("Unimplemented method");
		return false;
	#else
			string path;
		#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
			path = Application.persistentDataPath + "/localization/";
		#elif UNITY_EDITOR
			path = "StoredData/localization/";
		#else
			path = "StoredData/localization/";
		#endif
			string filePath = path + lang + "_" + sheetTitle + ".xml";
			return File.Exists(filePath);
	#endif
	}
	
	public string GetLanguageFileContents(string lang, string sheetTitle)
	{
	#if UNITY_WEBPLAYER
		Debug.LogWarning("Unimplemented method");
		return string.Empty;
	#else
			string path;
		#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
			path = Application.persistentDataPath + "/localization/";	
		#elif UNITY_EDITOR
			path = "StoredData/localization/";
		#else
			path = "StoredData/localization/";
		#endif
			if (!this.HasLanguageFile(lang, sheetTitle))
				return string.Empty;
				
			string file = path + lang + "_" + sheetTitle + ".xml";	
			return File.ReadAllText(file);
	#endif
	}
	
	/// <summary>
	/// Determines whether this instance is needed update. If has passed 24 hours from last synchronization, then update.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is needed update; otherwise, <c>false</c>.
	/// </returns>
	private bool IsNeededUpdate()
	{
		if (!PlayerPrefs.HasKey(LAST_UPDATE))
			return true;
		
		long temp = Convert.ToInt64(PlayerPrefs.GetString(LAST_UPDATE));
		DateTime lastUpdate = DateTime.FromBinary(temp);
		DateTime currentDate = System.DateTime.UtcNow;
		
		TimeSpan difference = currentDate.Subtract(lastUpdate);
//		Debug.Log(" - TIMESPAN: " + difference);
//		Debug.Log(" - TIMESPAN 2: " + currentDate.Subtract(lastUpdate));
		return (difference > TimeSpan.FromHours(Cache_Time_For_Update) ? true : false);
	}
	
	
	private System.Action<LocalizationServerManagerDelegateEventArgs> _onResult = null;
	
	/// <summary>
	/// Downloads the languages package.
	/// </summary>
	/// <param name='languageVersionOnline'>
	/// Language version online. 
	/// </param>
	/// <param name='onResult'>
	/// On result.
	/// </param>
	public void DownloadLanguages(System.Action<LocalizationServerManagerDelegateEventArgs> onResult = null)
	{
		if (!this.IsNeededUpdate())
		{
			RaiseLocalizationCallback (true, false, "it is not neccessary update the currents files", string.Empty, onResult);
		}
		else
		{
			DirectoryInfo dir = new DirectoryInfo(DataPath);
			if (!dir.Exists)
				dir.Create();
			
			_onResult = onResult;
			eventsListener.AMPSManagerInit += HandleAMPSListenerInstanceAMPSManagerInit;
			AMPSBinding.InitDMOAssetManager(eventsListener.name, DataPath);
		}
	}
	
	void HandleAMPSListenerInstanceAMPSManagerInit (object sender, AMPListenerEventArgs e)
	{
		eventsListener.AMPSManagerInit -= HandleAMPSListenerInstanceAMPSManagerInit;
		
		if (e.Result)
		{
			string serverFileVersion = AMPSBinding.GetVersionOfFile(DataPath, FILE_NAME);
			
			Debug.Log("Version local: " + PlayerPrefs.GetString(FILE_VERSION_KEY));
			Debug.Log("Version server: " + serverFileVersion);

			int versionLocal = string.IsNullOrEmpty(PlayerPrefs.GetString(FILE_VERSION_KEY)) ? 0 : Convert.ToInt32(PlayerPrefs.GetString(FILE_VERSION_KEY));
			int versionServer = string.IsNullOrEmpty(serverFileVersion) ? 0 : Convert.ToInt32(serverFileVersion);
			
			if (!PlayerPrefs.HasKey(FILE_VERSION_KEY))
			{
				eventsListener.FileDownloaded += HandleAMPSListenerInstanceFileDownloaded;
				AMPSBinding.DownloadAsset(DataPath, FILE_NAME);
			}
			else if (versionServer > versionLocal)
			{
				eventsListener.FileDownloaded += HandleAMPSListenerInstanceFileDownloaded;
				AMPSBinding.DownloadAsset(DataPath, FILE_NAME);
			}
			else
			{
				Debug.Log("Update is not neccessary");
				RaiseLocalizationCallback (true, false, "it is not neccessary update the currents files", string.Empty, _onResult);
			}
		}
		else
		{
			RaiseLocalizationCallback (false, false, "Error downloading " + e.FilenameDownloaded, e.Error, _onResult);
		}
	}
	
	void HandleAMPSListenerInstanceFileDownloaded (object sender, AMPListenerEventArgs e)
	{
		eventsListener.FileDownloaded -= HandleAMPSListenerInstanceFileDownloaded;
		
		Debug.Log("Asset downloaded " + e.Result);
		
		if (e.Result == true)
		{
			//Unzip the file
			bool succesfullUnzip = this.ExtractZipFile(this.GetPath(FILE_NAME));
			
			if (succesfullUnzip) 
			{
				PlayerPrefs.SetString(LAST_UPDATE, System.DateTime.UtcNow.ToBinary().ToString());
				PlayerPrefs.SetString(FILE_VERSION_KEY, AMPSBinding.GetVersionOfFile(DataPath, e.FilenameDownloaded));
				
				Language.SwitchLanguage(Language.CurrentLanguage());
			
				//Delete downloaded file
				File.Delete(this.GetPath(FILE_NAME));

				RaiseLocalizationCallback (true, true, e.Message, string.Empty, _onResult);
			}
			else {
				//Delete downloaded file
				File.Delete(this.GetPath(FILE_NAME));

				RaiseLocalizationCallback (false, false, "Error unzipping " + e.FilenameDownloaded, e.Error, _onResult);
			}
		}
		else
		{
			RaiseLocalizationCallback (false, false, "Error downloading " + e.FilenameDownloaded, e.Error, _onResult);
		}
		

	}

	#region Helpers	
	
	protected bool ExtractZipFile(string fileNameIn)
	{
	#if UNITY_WEBPLAYER
		Debug.LogWarning("Unimplemented method");
		return false;
	#else
		try
		{
			Debug.Log("Unziping file: " + fileNameIn);
			FileInfo fileInfo = new FileInfo(fileNameIn);
			
			using (ZipInputStream s = new ZipInputStream(fileInfo.OpenRead())) 
			{
				ZipEntry theEntry;
				while ((theEntry = s.GetNextEntry()) != null) {
					
//					Console.WriteLine(theEntry.Name);
					if (theEntry.IsDirectory)
						continue;
					
					string fileName = this.GetPath(Path.GetFileName(theEntry.Name.Replace("_temp", "")));
					
					if (fileName != String.Empty) {
						using (FileStream streamWriter = File.Create(fileName + "temp")) {
						
							int size = 2048;
							byte[] data = new byte[2048];
							while (true) {
								size = s.Read(data, 0, data.Length);
								if (size > 0) {
									streamWriter.Write(data, 0, size);
								} else {
									break;
								}
							}
						}
						
						if (File.Exists(fileName + "temp")) {
							if (File.Exists(fileName)) {
								File.Delete(fileName);
							}
							File.Move(fileName + "temp", fileName);
						}
					}
				}
			}
		}
		catch(Exception ex)
		{
			Debug.LogWarning("Error unzipping file. Exception: " + ex.Message);
			return false;
		}
		
		return true;
		#endif
	}
	
	private string GetPath(string objectId)
	{	
		string 	path2 		= DataPath + objectId;
		DirectoryInfo dir 	= new DirectoryInfo(DataPath);
		
		if (!dir.Exists)
			dir.Create();
		
		return path2;
	}
	
	
	#endregion
	
}
