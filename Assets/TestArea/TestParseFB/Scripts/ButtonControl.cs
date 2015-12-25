using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonControl : MonoBehaviour {
	
	private GameObject _saveUserButton;
	private GameObject _loginButton;
	private GameObject _modifyUser;
	private GameObject _saveToDisk;
	private GameObject _resetButton;
	private GameObject _reloadButton;
	private GameObject _localizationButton;
	private GameObject _tweaksButton;
	private UILabel _infoLabel;

	// Use this for initialization
	void Start () {
		
		_infoLabel = (UILabel)GameObject.Find("InfoLabel").GetComponent<UILabel>();
		
		_saveUserButton = GameObject.Find("ButtonSaveUser");
		_loginButton = GameObject.Find("ButtonLogin");
		_modifyUser = GameObject.Find("ButtonModifyUser");
		_saveToDisk = GameObject.Find("ButtonSaveToDiskUser");
		_resetButton = GameObject.Find("ButtonResetUser");
		_reloadButton = GameObject.Find("ButtonReloadUser");
		_localizationButton = GameObject.Find("ButtonLocalization");
		_tweaksButton = GameObject.Find("ButtonTweaks");
	}

	void OnClick() {
		if (gameObject == _saveUserButton) 
		{
			UserManagerCloud.Instance.SaveUserInCloud(result => 
			{
				switch (result.SaveIn) 
				{
					case ResultSave.Cloud:
						_infoLabel.text = "User save in: Disk and Cloud";
					break;
					case ResultSave.Disk:
						_infoLabel.text = "User save in: Disk";
					break;
					default:
					
					break;
				}
				
			});
		} 
		else if (gameObject == _loginButton) 
		{
			UserManagerCloud.Instance.InitUser( result => 
			{
				_infoLabel.text = result.Message;
			});
		} 
		else if (gameObject == _modifyUser) 
		{
			UserManagerCloud.Instance.ModifyUser( t =>
			{
				_infoLabel.text = "UserLocal Level: " + UserManagerCloud.Instance.CurrentUser.LastFinishedLvl;
				_infoLabel.text += "\nUserCloud Level: " + UserManagerCloud.Instance.CurrentCloudUser.LastFinishedLvl;
			});
			
		}
		else if (gameObject == _saveToDisk) 
		{
			UserManagerCloud.Instance.SaveDataToDisk();
		}
		else if (gameObject == _localizationButton)
		{
			LocalizationServerManager.Instance.DownloadLanguages( t =>
			{
				_infoLabel.text = t.Message;
			});
		}
		else if (gameObject == _tweaksButton)
		{
			TweaksSystemManager.Instance.SynchTweaks( t => 
			{
				_infoLabel.text = t.Message;
			});
		}
		else if (gameObject == _resetButton)
		{
			UserManagerCloud.Instance.ResetLocalUser();
			_infoLabel.text = "Level of Local User: " + UserManagerCloud.Instance.CurrentUser.LastFinishedLvl;
		}
		else if (gameObject == _reloadButton)
		{
			UserManagerCloud.Instance.DeleteUserFromCloud();
//			_userManager.LoadUserFromCloud( res =>
//			{
//				_infoLabel.text = "User loaded from iCloud: " + res.Result;
//			});
		}
	}
	
	// Only for test project. Show pictures profiles from Facebook
	public void OnGUI() {
		
//		if (avatar != null)
//        {
//            GUI.Label(new Rect(155, 350, avatar.width, avatar.height), avatar);
//        }
//		
//		if (renderFriendsPictures)
//		{	
//			int offset = 0;
//
//			foreach (User friend in User.CurrentUser.FacebookFriends) {
//				if (friend.Avatar == null)
//					continue;
//				GUI.Label(new Rect(155 + offset, 250, friend.Avatar.width, friend.Avatar.height), friend.Avatar);
//				offset += friend.Avatar.width + 5;
//			}
//		}
	}
	
}
