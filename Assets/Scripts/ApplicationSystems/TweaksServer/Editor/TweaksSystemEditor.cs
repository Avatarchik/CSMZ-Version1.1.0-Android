using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Security.Cryptography;

public class TweaksSystemEditor : EditorWindow {
	
	public string FILE_NAME = "tweaks.dat";
	public string path = Application.dataPath + "/StoredData";
	public string pathToBinary = Application.dataPath + "/StoredData";
	public string pathToJson = Application.dataPath + "/StoredData";
	
	
	[MenuItem("Project Tools/Tweaks System")]
    static void OpenWindow()
    {
        EditorWindow.GetWindow(typeof(TweaksSystemEditor));
    }	

	TweaksSystemEditor()
	{
	}
	
	Vector2 scrollview;
    void OnGUI()
	{
		GUILayout.Label("Settings", EditorStyles.boldLabel);
		scrollview = GUILayout.BeginScrollView(scrollview);
		
		///////////// Default values ///////////////
		
		GUILayout.BeginHorizontal();
		path = EditorGUILayout.TextField("Path to save current values", path);
		if (GUILayout.Button("Select"))
		{
			path = EditorUtility.OpenFolderPanel("Select path", path, "");
		}
		GUILayout.EndHorizontal();
		if (GUILayout.Button("Create Files With Default Values"))
		{
			if (!CreateDefaultValues(path))
			{
				EditorUtility.DisplayDialog("Error", "File was not created", "Ok");
			}

		}
		GUILayout.Space(25);
		/////////////////////////////////////////////
		
		
		//////////// Create binary file ////////////////////
		GUILayout.BeginHorizontal();
		pathToBinary = EditorGUILayout.TextField("Path to save Binary File", pathToBinary);
		if (GUILayout.Button("Select"))
		{
			pathToBinary = EditorUtility.OpenFolderPanel("Select path", pathToBinary, "");
		}
		GUILayout.EndHorizontal();
		if (GUILayout.Button("Create Binary From Json File"))
		{
			string selectedFile = "";
			selectedFile = EditorUtility.OpenFilePanel("Select json file", pathToBinary, "json");
			if (!GenerateBinaryFileWithFile(selectedFile, pathToBinary))
			{
				EditorUtility.DisplayDialog("Error", "File was not created", "Ok");
			}
		}
		
		GUILayout.Space(25);
		//////////////////////////////////////////////////////
		
		/////////////// Create Json file ///////////////////////
		GUILayout.BeginHorizontal();
		pathToJson = EditorGUILayout.TextField("Path to save Json File", pathToJson);
		if (GUILayout.Button("Select"))
		{
			pathToJson = EditorUtility.OpenFolderPanel("Select path", pathToJson, "");
		}
		GUILayout.EndHorizontal();
		if (GUILayout.Button("Create Json From Binary File"))
		{
			string selectedFile = "";
			selectedFile = EditorUtility.OpenFilePanel("Select dat file", path, "dat");
			if (!GenerateJsonFileWithFile(selectedFile, pathToJson))
			{
				EditorUtility.DisplayDialog("Error", "File was not created", "Ok");
			}
		}
		
	 	GUILayout.Space(25);
		///////////////////////////////////////////////////////////
		
		GUILayout.EndScrollView();
	}
	
	
	bool CreateDefaultValues(string path)
	{	
		try
		{
			bool result;
			
			Dictionary<string, object> root = new Dictionary<string, object>();
			root.Add("intValues", TweaksSystem.GetDefaultIntValues());
			root.Add("floatValues", TweaksSystem.GetDefaultFloatValues());
			root.Add("stringValues", TweaksSystem.GetDefaultStringValues());
			
			string jsonContent = JsonFx.Json.JsonWriter.Serialize(root);
			
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			
			bool save = true;
			if (File.Exists(path + "/tweaks.json") && EditorUtility.DisplayDialogComplex("", "Already exists tweaks.json file inside target folder. Do you like overwrite it?", "OK", "Cancel", "") == 1)
			{
				save = false;
			}
			if (save)
			{
				File.WriteAllText(path + "/tweaks.json", jsonContent); 
				result = GenerateBinaryFileWithFile(path + "/tweaks.json", path);
				
				return result;
			}
			else
			{
				return false;
			}

		}
		catch(Exception ex)
		{
			Debug.Log("Error creating json. Message: " + ex.Message);
			return false;
		}
		
	}
	
	bool GenerateBinaryFileWithFile(string filePath, string insidePath)
	{
		if (File.Exists(filePath))
		{
			string jsonContent = File.ReadAllText(filePath);
			Dictionary<string, object> root = (Dictionary<string, object>)JsonFx.Json.JsonReader.Deserialize(jsonContent);
			Dictionary<string, object> intValues = (Dictionary<string, object>)root["intValues"];
			Dictionary<string, object> floatValues = (Dictionary<string, object>)root["floatValues"];
			Dictionary<string, object> stringValues = (Dictionary<string, object>)root["stringValues"];
			
			string newBinaryFile = insidePath + "/" + FILE_NAME;
			
			bool cont = true;
			if (File.Exists(newBinaryFile) && EditorUtility.DisplayDialogComplex("", "Already exists " + FILE_NAME  + " file inside target folder. Do you like overwrite it?", "OK", "Cancel", "") == 1)
			{
				cont = false;
			}
			if (!cont)
				return false;
		
//			try
//			{
//				Debug.Log("KEY: " + TweaksSystem.CRYPTO_KEY);
//				
				MemoryStream ms = new MemoryStream();
//				DESCryptoServiceProvider mDES = new DESCryptoServiceProvider();
//				mDES.Mode = CipherMode.ECB;
//				mDES.Key = System.Text.Encoding.UTF8.GetBytes(TweaksSystem.CRYPTO_KEY);
//				
//				CryptoStream encStream = new CryptoStream(ms, mDES.CreateEncryptor(), CryptoStreamMode.Write);
				
				using (BinaryWriter writeStream = new BinaryWriter(ms))
				{
					if (stringValues != null)
					{
						writeStream.Write(stringValues.Count);
						foreach (KeyValuePair<string, object> item in stringValues) 
						{
							writeStream.Write(item.Key);
							writeStream.Write((string)item.Value);
						}
					}
					else
						writeStream.Write(0);
					
					
					if (intValues != null)
					{
						writeStream.Write(intValues.Count);
						foreach (KeyValuePair<string, object> item in intValues) 
						{
							writeStream.Write(item.Key);
							writeStream.Write(Convert.ToInt32(item.Value));
						}
					}
					else
						writeStream.Write(0);
					
						
					if (floatValues != null)
					{
						writeStream.Write(floatValues.Count);
						foreach (KeyValuePair<string, object> item in floatValues) 
						{
							writeStream.Write(item.Key);
							writeStream.Write(Convert.ToSingle(item.Value));
						}
					}
					else
						writeStream.Write(0);
					
					writeStream.Close();
				}
//				encStream.Close();
				ms.Close();
			
				using (FileStream file = new FileStream(newBinaryFile, FileMode.Create))
				{
					byte[] res = ms.ToArray();
					file.Write(res, 0, res.Length);
				}
				
//			}
//			catch (Exception ex)
//			{
//				Debug.Log("Error creating binary data. Message: " + ex.Message);
//				return false;
//			}
			
			OpenFolder (insidePath);
				
			return true;
		}
		else
			return false;
	}
	
	bool GenerateJsonFileWithFile(string filePath, string insidePath)
	{
		Dictionary<string, string>	stringValues = new Dictionary<string, string>();
		Dictionary<string, float>	floatValues = new Dictionary<string, float>();
		Dictionary<string, int> 	intValues = new Dictionary<string, int>();
		
		if (File.Exists(filePath))
		{
//			try
//			{
				Stream str = File.Open(filePath, FileMode.Open);
				
//				Debug.Log("KEY: " + TweaksSystem.CRYPTO_KEY);
//				
//				DESCryptoServiceProvider cryptic = new DESCryptoServiceProvider();
//				cryptic.Mode = CipherMode.ECB;
//				cryptic.Key = System.Text.Encoding.UTF8.GetBytes(TweaksSystem.CRYPTO_KEY);
//				
//				CryptoStream crStream = new CryptoStream(str, cryptic.CreateDecryptor(), CryptoStreamMode.Read);
				
				using (BinaryReader readerStream = new BinaryReader(str))
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
					Debug.Log("Disk - Strings: " + stringValues.Count);
				
					int intCount = readerStream.ReadInt32();
					Debug.Log("Disk - Ints: " + intValues.Count);
					for(int i = 0; i < intCount; i++) 
					{
						string key = readerStream.ReadString();
						int val = readerStream.ReadInt32();
						Debug.Log("Disk - Ints[" + i + "] : " + key + " = " + val);
						if (intValues.ContainsKey(key))
							intValues[key] = val;
						else
							intValues.Add(key, val);
					}
					Debug.Log("Disk - Ints: " + intValues.Count);
				
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
					Debug.Log("Disk - Floats: " + floatValues.Count);
					
					readerStream.Close();
//					crStream.Close();
					
				}
				str.Close();
			
				Dictionary<string, object> root = new Dictionary<string, object>();
				root.Add("intValues", intValues);
				root.Add("floatValues", floatValues);
				root.Add("stringValues", stringValues);
			
				string jsonContent = JsonFx.Json.JsonWriter.Serialize(root);
			
				if (!Directory.Exists(insidePath))
					Directory.CreateDirectory(insidePath);
				
				bool save = true;
			
				if (File.Exists(insidePath + "/tweaks.json") && EditorUtility.DisplayDialogComplex("", "Already exists " + "tweaks.json"  + " file inside target folder. Do you like overwrite it?", "OK", "Cancel", "") == 1)
				{	
					save = false;
				}
				if (save)
				{
				  	File.WriteAllText(insidePath + "/tweaks.json", jsonContent); 
					OpenFolder (insidePath);
					return true;
				}
				else
					return false;
				
//			}
//			catch (Exception ex)
//			{
//				Debug.Log("Error creating json file. Exception: " + ex.Message);
//				return false;
//			}
		}
		else
		{
			return false;
		}
				
	}
	
	
	void OpenFolder (string insidePath)
	{
		string arguments = "" + insidePath;
		//Debug.Log("arguments: " + arguments);
		try
		{
			System.Diagnostics.Process.Start("open", arguments);
		}
		catch(System.ComponentModel.Win32Exception e)
		{
			// tried to open mac finder in windows
			// just silently skip error
			// we currently have no platform define for the current OS we are in, so we resort to this
			e.HelpLink = ""; // do anything with this variable to silence warning about not using it
		}
	}
}
