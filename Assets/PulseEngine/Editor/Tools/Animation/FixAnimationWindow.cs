	using UnityEngine;



	using UnityEditor;



	using System.Collections;



	using System.Collections.Generic;



	public class FixAnimationWindow : EditorWindow



	{



	    List<Animation> m_animations = new List<Animation>();



	    Object[] m_oldObjects = null;



	    System.DateTime m_lastFrameTime;



	    float m_totalTime = 0f;



	    int cIndex = 0;



	    [MenuItem("Mobility Games/Animations/FixAnimation")]



	    static void Init()



	    {



	        //create new window



	       EditorWindow.GetWindow<FixAnimationWindow>(false, "FixAnimation", true);



	    }



	    void OnInspectorUpdate()



	    {



	        this.Repaint();



	    }



	    void OnSelectionChange()



	    {



	        m_animations.Clear();



	        if( Selection.activeGameObject == null )



	        {



	            return;



	        }



	        foreach(GameObject go in Selection.gameObjects)



	        {



	            Animation ani = go.GetComponent<Animation>();



	            if(ani)



	            {



	                AnimationClip[] clips = AnimationUtility.GetAnimationClips(ani);



	                if(clips.Length > 0)



	                {



	                    m_animations.Add(ani);



	                }



	            }



	        }



	    }



	    void OnGUI()



	    {



	        if(Selection.activeGameObject != null && m_animations.Count == 0)



	        {



	            OnSelectionChange();



	        }



	        if(Selection.activeGameObject == null || m_animations.Count == 0)



	        {



	            GUILayout.Label( "Must select one GameObject with Animation" );



	            return;



	        }



	        GUILayout.Label( string.Format("You selected {0} animation(s)", m_animations.Count ) );



	        if(GUILayout.Button("Fix Animation Length"))



	        {



	            Fix();



	        }



	    }



	    // Update is called once per frame



	    void Update ()



	    {



	        //re-select old objects to force repainting AnimationWindow



	        if(m_oldObjects != null)



	        {



	            m_totalTime += (float)System.DateTime.Now.Subtract(m_lastFrameTime).TotalSeconds;



	            m_lastFrameTime = System.DateTime.Now;



	            if(m_totalTime >= 0.1f)



	            {



	                Selection.objects = m_oldObjects;



	                m_oldObjects = null;



	            }



	        }



	    }



	    void Fix()



	    {



	        foreach(Animation ani in m_animations)



	        {



	            AnimationClip[] clips = AnimationUtility.GetAnimationClips(ani);



	            foreach(AnimationClip clip in clips)



	            {



	                AnimationClipCurveData[] curves = AnimationUtility.GetAllCurves(clip);



	                foreach (AnimationClipCurveData currentCurveData in curves) {



	                    string propName = currentCurveData.propertyName;

	                    if (propName == "m_LocalEulerAnglesHint.x") {

	                         AnimationUtility.SetEditorCurve(clip, currentCurveData.path, typeof(UnityEngine.Transform), "m_LocalEulerAnglesHint.x", null);

	                        cIndex ++;



	                    } else if (propName == "m_LocalEulerAnglesHint.y") {
	                        AnimationUtility.SetEditorCurve(clip, currentCurveData.path, typeof(UnityEngine.Transform), "m_LocalEulerAnglesHint.y", null);
	                        cIndex ++;
	                    } else if (propName == "m_LocalEulerAnglesHint.z") {
	                        AnimationUtility.SetEditorCurve(clip, currentCurveData.path, typeof(UnityEngine.Transform), "m_LocalEulerAnglesHint.z", null);
	                        cIndex ++;
	                    }
	                }
	            }

	            if (cIndex != 0) {
	                Debug.Log(cIndex + "false items cleared!");
	            } else {
	                Debug.Log("Nothing to clear!");
	            }
	        }


	        if(AnimationUtility.InAnimationMode())
	        {
	            AnimationUtility.StopAnimationMode();
	        }
	        else
	        {
	            //re-select objects to force repainting AnimationWindow
	            m_oldObjects = Selection.objects;
	            Selection.objects = new Object[0];
	            m_lastFrameTime = System.DateTime.Now;
	            m_totalTime = 0;
	        }
	    }
	}

