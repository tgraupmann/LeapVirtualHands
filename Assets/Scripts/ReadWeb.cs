using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadWeb : MonoBehaviour
{

    public string _mIPAddress = string.Empty;
    public GameObject _mThumb = null;
    public GameObject _mIndex = null;
    public GameObject _mMiddle = null;
    public GameObject _mRing = null;
    public GameObject _mPinky = null;
    private FingerData _mFingerData = null;
    private FingerData _mStraight = new FingerData();
    private FingerData _mFist = new FingerData();
    private Dictionary<GameObject, Vector3> _mOriginalEulers = new Dictionary<GameObject, Vector3>();
    private const string KEY_STRAIGHT = "Straight";
    private const string KEY_FIST = "Fist";

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
        // save original eulers
        SetOriginalEulers(_mThumb);
        SetOriginalEulers(_mIndex);
        SetOriginalEulers(_mMiddle);
        SetOriginalEulers(_mRing);
        SetOriginalEulers(_mPinky);

        if (string.IsNullOrEmpty(_mIPAddress))
        {
            yield break;
        }

        if (PlayerPrefs.HasKey(KEY_STRAIGHT))
        {
            string json = PlayerPrefs.GetString(KEY_STRAIGHT);
            if (!string.IsNullOrEmpty(json))
            {
                _mStraight = JsonUtility.FromJson<FingerData>(json);
            }
        }
        if (PlayerPrefs.HasKey(KEY_FIST))
        {
            string json = PlayerPrefs.GetString(KEY_FIST);
            if (!string.IsNullOrEmpty(json))
            {
                _mFist = JsonUtility.FromJson<FingerData>(json);
            }
        }

        StartCoroutine(DoRequests());
    }

    IEnumerator DoRequests()
    {
        while (true)
        {
            string url = string.Format("http://{0}", _mIPAddress);
            DateTime timeout = DateTime.Now + TimeSpan.FromMilliseconds(500);
            WWW www = new WWW(url);
            while (DateTime.Now < timeout &&
                string.IsNullOrEmpty(www.error) &&
                !www.isDone)
            {
                yield return null;
            }
            string json = null;
            if (timeout <= DateTime.Now)
            {
                Debug.LogError("WWW Timeout");
            }
            else if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError(www.error);
            }
            else if (www.isDone)
            {
                json = www.text;
            }
            www.Dispose();
            if (!string.IsNullOrEmpty(json))
            {
                //Debug.Log(json);
                FingerData fingerData = JsonUtility.FromJson<FingerData>(json);
                if (null != fingerData)
                {
                    _mFingerData = fingerData;
                }
            }
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
            if (null != _mStraight)
            {
                PlayerPrefs.SetString(KEY_STRAIGHT, JsonUtility.ToJson(_mStraight));
            }
        }
        if (GUILayout.Button("Calibrate Fist", GUILayout.Height(60)))
        {
            _mFist = _mFingerData;
            if (null != _mFist)
            {
                PlayerPrefs.SetString(KEY_FIST, JsonUtility.ToJson(_mFist));
            }
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

    void SetOriginalEulers(GameObject go)
    {
        if (go)
        {
            _mOriginalEulers[go] = go.transform.localRotation.eulerAngles;
        }
    }

    Vector3 GetOriginalEulers(GameObject go)
    {
        if (go &&
            _mOriginalEulers.ContainsKey(go))
        {
            return _mOriginalEulers[go];
        }
        else
        {
            return Vector3.zero;
        }
    }

    float GetInverseLerpThumb()
    {
        if (null == _mStraight ||
            null == _mFist ||
            null == _mFingerData)
        {
            return 0f;
        }
        return Mathf.InverseLerp(_mStraight.thumb, _mFist.thumb, _mFingerData.thumb);
    }

    float GetInverseLerpIndex()
    {
        if (null == _mStraight ||
            null == _mFist ||
            null == _mFingerData)
        {
            return 0f;
        }
        return Mathf.InverseLerp(_mStraight.index, _mFist.index, _mFingerData.index);
    }

    float GetInverseLerpMiddle()
    {
        if (null == _mStraight ||
            null == _mFist ||
            null == _mFingerData)
        {
            return 0f;
        }
        return Mathf.InverseLerp(_mStraight.middle, _mFist.middle, _mFingerData.middle);
    }

    float GetInverseLerpRing()
    {
        if (null == _mStraight ||
            null == _mFist ||
            null == _mFingerData)
        {
            return 0f;
        }
        return Mathf.InverseLerp(_mStraight.ring, _mFist.ring, _mFingerData.ring);
    }

    float GetInverseLerpPinky()
    {
        if (null == _mStraight ||
            null == _mFist ||
            null == _mFingerData)
        {
            return 0f;
        }
        return Mathf.InverseLerp(_mStraight.pinky, _mFist.pinky, _mFingerData.pinky);
    }

    private void Update()
    {
        if (_mThumb)
        {
            Vector3 eulers = GetOriginalEulers(_mThumb);
            float t = GetInverseLerpThumb();
            eulers.z -= Mathf.Lerp(0, 90, t);
            _mThumb.transform.localRotation = Quaternion.Euler(eulers);
        }

        if (_mIndex)
        {
            Vector3 eulers = GetOriginalEulers(_mIndex);
            float t = GetInverseLerpIndex();
            eulers.z -= Mathf.Lerp(0, 90, t);
            _mIndex.transform.localRotation = Quaternion.Euler(eulers);
        }

        if (_mMiddle)
        {
            Vector3 eulers = GetOriginalEulers(_mMiddle);
            float t = GetInverseLerpMiddle();
            eulers.z -= Mathf.Lerp(0, 90, t);
            _mMiddle.transform.localRotation = Quaternion.Euler(eulers);
        }

        if (_mRing)
        {
            Vector3 eulers = GetOriginalEulers(_mRing);
            float t = GetInverseLerpRing();
            eulers.z -= Mathf.Lerp(0, 90, t);
            _mRing.transform.localRotation = Quaternion.Euler(eulers);
        }

        if (_mPinky)
        {
            Vector3 eulers = GetOriginalEulers(_mPinky);
            float t = GetInverseLerpPinky();
            eulers.z -= Mathf.Lerp(0, 90, t);
            _mPinky.transform.localRotation = Quaternion.Euler(eulers);
        }
    }
}
