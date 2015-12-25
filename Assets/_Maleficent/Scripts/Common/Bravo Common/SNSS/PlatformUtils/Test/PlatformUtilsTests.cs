using UnityEngine;
using System.Collections;

public class PlatformUtilsTests : MonoBehaviour {

	public TextMesh textMeshBundleVersion;
	public TextMesh textMeshBundleID;

	void Awake()
	{
		textMeshBundleVersion.text = PlatformUtilsManager.GetBundleVersion();
		textMeshBundleID.text = PlatformUtilsManager.GetBundleID();
	}
}
