using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadWeb : MonoBehaviour
{

    public string _mIPAddress = string.Empty;
    private FingerData _mFingerData = null;
    private FingerData _mStraight = null;
    private FingerData _mFist = null;

    [System.Serializable]
    public class FingerData
    {
        public int thumb;
        public int index;
        public int middle;
        public int ring;
        public int pinky;
    }

    IEnumerator Start()
    {
        if (string.IsNullOrEmpty(_mIPAddress))
        {
            yield break;
        }
        while (true)
        {
            string url = string.Format("http://{0}", _mIPAddress);
            WWW www = new WWW(url);
            yield return www;
            string json = www.text;
            www.Dispose();
            //Debug.Log(json);
            _mFingerData = JsonUtility.FromJson<FingerData>(json);
            yield return null;
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical(GUILayout.Height(Screen.height));

        GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
        if (GUILayout.Button("Calibrate Straight", GUILayout.Height(60)))
        {
            _mStraight = _mFingerData;
        }
        if (GUILayout.Button("Calibrate Fist", GUILayout.Height(60)))
        {
            _mFist = _mFingerData;
        }
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        if (null != _mFingerData)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
            GUILayout.Label(string.Format("Thumb: {0}", _mFingerData.thumb));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
            GUILayout.Label(string.Format("Index: {0}", _mFingerData.index));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
            GUILayout.Label(string.Format("Middle: {0}", _mFingerData.middle));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
            GUILayout.Label(string.Format("Ring: {0}", _mFingerData.ring));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
            GUILayout.Label(string.Format("Pinky: {0}", _mFingerData.pinky));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
    }
}
