using UnityEngine;
using System.Collections;

public class ManaStoreScrollClipper : MonoBehaviour {
	private Camera uiCamera;
	private UIPanel panel;
	private UIDraggablePanel draggablePanel;
	private UIRoot uiRoot;


	void Awake () 
	{
		panel = GetComponent<UIPanel>();
		draggablePanel = GetComponent<UIDraggablePanel>();
		uiCamera = NGUITools.FindCameraForLayer(gameObject.layer);
		uiRoot = NGUITools.FindInParents<UIRoot>(gameObject);
	}

	void Start ()
	{
		UpdateClipRange();
		OrientationListener.Instance.OnOrientationChanged += HandleOnOrientationChanged;
	}

	void OnDestroy ()
	{
		if(OrientationListener.IsInstantiated()) {
			OrientationListener.Instance.OnOrientationChanged -= HandleOnOrientationChanged;
		}
	}

	void UpdateClipRange ()
	{

		Rect pixelRect = uiCamera.pixelRect;
		float rectWidth = pixelRect.width;
		float rectHeight = pixelRect.height;
		float scale = uiRoot.activeHeight / rectHeight;

		Bounds dragginBounds = draggablePanel.bounds;
		Vector4 clipRange = panel.clipRange;
		clipRange.z = rectWidth * scale;

		if(dragginBounds.size.x > clipRange.z) {
			draggablePanel.relativePositionOnReset = new Vector2(0, 0);
		}else {
			draggablePanel.relativePositionOnReset = new Vector2(0.5f, 0.5f);
		}

		panel.clipRange = clipRange;
		draggablePanel.ResetPosition();
	}

	void HandleOnOrientationChanged (ScreenOrientation newOrientation)
	{
		UpdateClipRange();
	}
}
