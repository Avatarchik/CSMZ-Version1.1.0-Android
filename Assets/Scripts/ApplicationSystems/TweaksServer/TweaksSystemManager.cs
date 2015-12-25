using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;

public enum TweaksResultSave
{
	Disk,
	NotSave
}

public class TweaksSystemEventArgs : EventArgs
{
	public bool Result {get; set; }
	public string Message {get; set;}
	public string Error {get; set;}
	public TweaksResultSave SaveIn {get; set;}
}

public class TweaksSystemManager
{
	public const string FILE_NAME = "tweaks.dat";
	public const string FILE_VERSION_KEY = "tweaks_version";
	private const string LAST_UPDATE = "LastTweaksSyncrhonization";
	
	public int Cache_Time_For_Update = 0; // In hours
	
	public AMPSListener eventsListener;
	
	Dictionary<string, int> intValues;
	Dictionary<string, float> floatValues;
	Dictionary<string, string> stringValues;	
	
	#region Init
	
	private static TweaksSystemManager instance;
	public static TweaksSystemManager Instance
	{
		get
		{
			if (instance == null) {
				instance = new TweaksSystemManager();
			}
			
			return instance;
		}
	}
	
	public TweaksSystemManager()
	{
		eventsListener = AMPSListener.CreateInstance("TweaksAmpsListener");
		
		intValues = new Dictionary<string, int>();
		floatValues = new Dictionary<string, float>();
		stringValues = new Dictionary<string, string>();
	}
	
	
	#endregion
	
	// Deprecated
	/// <summary>
	/// Saves the in cloud. Only for test proposal or Fill the table with default values. Comunication must be unidirectional.
	/// </summary>
//	private void SaveInCloud()
//	{
//		Dictionary<string, int> intValues = TweaksSystem.Instance.intValues;
//		Dictionary<string, float> floatValues = TweaksSystem.Instance.floatValues;
//		Dictionary<string, string> stringVlues = TweaksSystem.Instance.stringValues;
//	
//		List<ParseObject> objectToSave = new List<ParseObject>();
//		
//		if (stringVlues != null)
//		{
//			foreach (KeyValuePair<string, string> item in stringVlues) {
//				ParseObject tweaks = ParseObject.Create("TweaksSystem");
//				tweaks["key"] = item.Key;
//				tweaks["value"] = item.Value;
//				objectToSave.Add(tweaks);
//			}
//		}
//		
//		if (intValues != null)
//		{
//			foreach (KeyValuePair<string, int> item in intValues) {
//				ParseObject tweaks = ParseObject.Create("TweaksSystem");
//				tweaks["key"] = item.Key;
//				tweaks["value"] = item.Value.ToString();
//				objectToSave.Add(tweaks);
//			}
//		}
//		
//		if (floatValues != null)
//		{
//			foreach (KeyValuePair<string, float> item in floatValues) {
//				ParseObject tweaks = ParseObject.Create("TweaksSystem");
//				tweaks["key"] = item.Key;
//				tweaks["value"] = item.Value.ToString("0.0000");
//				objectToSave.Add(tweaks);
//			}
//		}
//		
//		ParseObject.SaveAllAsync(objectToSave).ContinueWith( t =>
//		{
//			Debug.Log("Object saved " + t.IsCompleted);
//		});
//	}
	
	
	private void SetNewValues ()
	{
		Debug.Log("TweaksSystemManager. Setting new values");
		
		foreach (KeyValuePair<string, int> item in intValues)
		{
			//Debug.Log("Tweak System: " + item.Key + " = " + item.Value);
			if (TweaksSystem.Instance.intValues.ContainsKey(item.Key))
				TweaksSystem.Instance.intValues[item.Key] = item.Value;
			else
				TweaksSystem.Instance.intValues.Add(item.Key, item.Value);
		}
		foreach (KeyValuePair<string, float> item in floatValues)
		{
			if (TweaksSystem.Instance.floatValues.ContainsKey(item.Key))
				TweaksSystem.Instance.floatValues[item.Key] = item.Value;
			else
				TweaksSystem.Instance.floatValues.Add(item.Key, item.Value);
		}
		foreach (KeyValuePair<string, string> item in stringValues)
		{
			if (TweaksSystem.Instance.stringValues.ContainsKey(item.Key))
				TweaksSystem.Instance.stringValues[item.Key] = item.Value;
			else
				TweaksSystem.Instance.stringValues.Add(item.Key, item.Value);
		}
	}
	
	// Deprecated. DO NOT USE
	/// <summary>
	/// Saves Tweaks the in disk.
	/// </summary>
	private void SaveInDisk()
	{	
		string path = this.GetPath(FILE_NAME);
		
		try
		{
			using (BinaryWriter writeStream = new BinaryWriter(File.Create(path)))
			{
				writeStream.Write(stringValues.Count);
				foreach (KeyValuePair<string, string> item in stringValues) 
				{
					writeStream.Write(item.Key);
					writeStream.Write(item.Value);
				}
				
				writeStream.Write(intValues.Count);
				foreach (KeyValuePair<string, int> item in intValues) 
				{
					writeStream.Write(item.Key);
					writeStream.Write(item.Value);
				}
				
				writeStream.Write(floatValues.Count);
				foreach (KeyValuePair<string, float> item in floatValues) 
				{
					writeStream.Write(item.Key);
					writeStream.Write(item.Value);
				}
			}
			
			SetNewValues ();
			
			PlayerPrefs.SetString(LAST_UPDATE, System.DateTime.UtcNow.ToBinary().ToString());
			PlayerPrefs.Save();
		}
		catch(Exception ex)
		{
			Debug.Log("Error creating saving file in disk. Exception: " + ex.Message);
		}
	}
	
	/// <summary>
	/// Loads tweaks from disk.
	/// </summary>
	public void LoadFromDisk()
	{
		string path = this.GetPath(FILE_NAME);
		
//		Dictionary<string, string> stringDict = new Dictionary<string, string>();
//		Dictionary<string, int> intDict = new Dictionary<string, int>();
//		Dictionary<string, float> floatDict = new Dictionary<string, float>();
		if (stringValues == null)
			stringValues = new Dictionary<string, string>();
		if (floatValues == null)
			floatValues = new Dictionary<string, float>();
		if (intValues == null)
			intValues = new Dictionary<string, int>();
		
		BinaryReader readerStream = null;
		Stream str = null;
		
		bool error = false;

		if (File.Exists(path))
		{
			try
			{
				str = File.Open(path, FileMode.Open);
//				DESCryptoServiceProvider cryptic = new DESCryptoServiceProvider();
//				cryptic.Mode = CipherMode.ECB;
//				cryptic.Key = System.Text.Encoding.UTF8.GetBytes(TweaksSystem.CRYPTO_KEY);
//				
//				CryptoStream crStream = new CryptoStream(str, cryptic.CreateDecryptor(), CryptoStreamMode.Read);
				
				
				using (readerStream = new BinaryReader(str))
				{
					int stringCount = readerStream.ReadInt32();
					for(int i = 0; i < stringCount; i++) 
					{
						string key = readerStream.ReadString();
						string val = readerStream.ReadString();
						if (stringValues.ContainsKey(key))
							stringValues[key] = val;
						else
							stringValues.Add(key, val);
					}
					//Debug.Log("Disk - Strings: " + stringValues.Count);
				
					int intCount = readerStream.ReadInt32();
					//Debug.Log("Disk - Ints: " + intValues.Count);
					for(int i = 0; i < intCount; i++) 
					{
						string key = readerStream.ReadString();
						int val = readerStream.ReadInt32();
						//Debug.Log("Disk - Ints[" + i + "] : " + key + " = " + val);
						if (intValues.ContainsKey(key))
							intValues[key] = val;
						else
							intValues.Add(key, val);
					}
					//Debug.Log("Disk - Ints: " + intValues.Count);
				
					int floatCount = readerStream.ReadInt32();
					for(int i = 0; i < floatCount; i++) 
					{
						string key = readerStream.ReadString();
						float val = readerStream.ReadSingle();
						if (floatValues.ContainsKey(key))
							floatValues[key] = val;
						else
							floatValues.Add(key, val);
					}
					//Debug.Log("Disk - Floats: " + floatValues.Count);
					readerStream.Close();
//					crStream.Close();
				}
				str.Close();
				
			
				this.SetNewValues();
			}
			catch (Exception ex)
			{
				Debug.Log("Error loading tweaks file from disk. Exception: " + ex.Message);
				error = true;
			}
			finally
			{
				if (readerStream != null)
				{
					readerStream.Close();
				}
				
				if (str != null)
				{
					str.Close();
				}
				
				if (error) {
					try {
						File.Delete(path); //this file is corrupted
					}
					catch (Exception exx) {
						Debug.Log("Error at deleting tweaks file from disk. Exception: " + exx.Message);
					}
				}
			}
		}
	}
	
	/// <summary>
	/// Synchs the tweaks.
	/// </summary>
	/// <param name='onResult'>
	/// Info with result of action
	/// </param>
	private System.Action<TweaksSystemEventArgs> _onResult = null;
	public void SynchTweaks(System.Action<TweaksSystemEventArgs> onResult = null)
	{
		if (!this.IsNeededUpdate())
		{
			Debug.Log("Update is not neccessary");
			TweaksSystemEventArgs args = new TweaksSystemEventArgs();
			args.Result = true;
			args.Message = "Update is not neccessary, loaded default values";
			args.SaveIn = TweaksResultSave.NotSave;
			if (onResult != null)
				onResult(args);
			
			this.LoadFromDisk();
			
			return;
		}
		
		_onResult = onResult;
		eventsListener.AMPSManagerInit += HandleAMPSListenerInstanceAMPSManagerInit;
		AMPSBinding.InitDMOAssetManager(eventsListener.name, GetPath());
	}

	void HandleAMPSListenerInstanceAMPSManagerInit (object sender, AMPListenerEventArgs e)
	{
		eventsListener.AMPSManagerInit -= HandleAMPSListenerInstanceAMPSManagerInit;
		
		string serverFileVersion = AMPSBinding.GetVersionOfFile(GetPath(), FILE_NAME);
		
		Debug.Log("TWEAKS Version " + serverFileVersion);
		if (e.Result)
		{
			if (!PlayerPrefs.HasKey(FILE_VERSION_KEY))
			{
				eventsListener.FileDownloaded += HandleAMPSListenerInstanceFileDownloaded;
				AMPSBinding.DownloadAsset(GetPath(), FILE_NAME);
			}
			else if (Convert.ToSingle(serverFileVersion) > Convert.ToSingle(PlayerPrefs.GetString(FILE_VERSION_KEY)))
			{
				eventsListener.FileDownloaded += HandleAMPSListenerInstanceFileDownloaded;
				AMPSBinding.DownloadAsset(GetPath(), FILE_NAME);
			}
			else
			{
				Debug.Log("Update is not neccessary");
				TweaksSystemEventArgs args = new TweaksSystemEventArgs();
				args.Result = true;
				args.Message = "Update is not neccessary, loaded default values";
				args.SaveIn = TweaksResultSave.NotSave;
			
				this.LoadFromDisk();
			}
		}
		else
		{
			TweaksSystemEventArgs res = new TweaksSystemEventArgs();
			res.Error = e.Error;
			res.Message = "Error downloading " + e.FilenameDownloaded;
			res.SaveIn = TweaksResultSave.NotSave;
			res.Result = false;
			if (_onResult != null) 
			{	
				_onResult(res);
			}
			
			this.LoadFromDisk();
		}
	}

	void HandleAMPSListenerInstanceFileDownloaded (object sender, AMPListenerEventArgs e)
	{
		eventsListener.FileDownloaded -= HandleAMPSListenerInstanceFileDownloaded;
		
		Debug.Log("Asset downloaded " + e.Result);
		TweaksSystemEventArgs res = new TweaksSystemEventArgs();
		if (e.Result == true)
		{
			PlayerPrefs.SetString(LAST_UPDATE, System.DateTime.UtcNow.ToBinary().ToString());
			PlayerPrefs.SetString(FILE_VERSION_KEY, AMPSBinding.GetVersionOfFile(GetPath(), e.FilenameDownloaded));
			this.LoadFromDisk();
			res.Error = null;
			res.Message = "Tweaks System Downloaded and setted as new values";
			res.SaveIn = TweaksResultSave.Disk;
			res.Result = true;
			
			PlayerPrefs.Save();
		}
		else
		{
			res.Error = e.Error;
			res.Message = "Error downloading " + e.FilenameDownloaded;
			res.SaveIn = TweaksResultSave.NotSave;
			res.Result = false;
			
			this.LoadFromDisk();
		}
		
		if (_onResult != null) 
		{	
			_onResult(res);
		}
	}

	
	
	#region Helpers
	
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
		return (difference > TimeSpan.FromHours(Cache_Time_For_Update) ? true : false);
	}
	
	private string GetPath(string objectId = null)
	{	
		string path;
	#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
		path = Application.persistentDataPath + "/tweaks/";
	#elif UNITY_EDITOR
		path = "StoredData/tweaks/";
	#else
		path = "StoredData/tweaks/";
	#endif
		
		DirectoryInfo dir = new DirectoryInfo(path);
		if (!dir.Exists)
			dir.Create();
		
		if (string.IsNullOrEmpty(objectId))
			return path;
		
		return path + objectId;
	}
	
	#endregion
	

	
}

