using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ReadSerialPort : MonoBehaviour
{
    public string _mPortName = "COM4";
    public string _mBaudRate = "9600";

    public GameObject _mJoint = null;

    public int _mRotation = 0;
    int _mJointStraight = 715;
    int _mJointDown = 906;

    private Vector3 _mOriginalJoint;

    Process _mProcess = null;

    // Use this for initialization
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (_mJoint)
        {
            _mOriginalJoint = _mJoint.transform.localRotation.eulerAngles;
        }
    }

    // Update is called once per frame
    IEnumerator Start()
    {
        _mProcess = new Process();
        try
        {
            _mProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            _mProcess.StartInfo.CreateNoWindow = true;
            _mProcess.StartInfo.UseShellExecute = false;
            _mProcess.StartInfo.FileName = "ArduinoReadSerialConsole.exe";
            _mProcess.StartInfo.Arguments = string.Format("{0} {1}", _mPortName, _mBaudRate);
            _mProcess.StartInfo.RedirectStandardOutput = true;
            _mProcess.EnableRaisingEvents = true;
            bool result = _mProcess.Start();
            if (!result)
            {
                Debug.LogError("Process failed to start");
            }
            
        }
        catch (Exception e)
        {
             Debug.LogError(e);
        }

        while (!_mProcess.HasExited)
        {
            string line = _mProcess.StandardOutput.ReadLine();
            if (!string.IsNullOrEmpty(line))
            {
                if (int.TryParse(line, out _mRotation))
                {
                    if (_mJoint)
                    {
                        Vector3 eulers = _mOriginalJoint;
                        float t = Mathf.InverseLerp(_mJointStraight, _mJointDown, _mRotation);
                        eulers.z -= Mathf.Lerp(0, 90, t);
                        _mJoint.transform.localRotation = Quaternion.Euler(eulers);
                    }
                }
                //Debug.Log(line);
            }
            yield return null;
        }
        //Debug.Log("Process has exited.");
    }

    private void OnApplicationQuit()
    {
        if (!_mProcess.HasExited)
        {
            _mProcess.Kill();
            _mProcess.Close();
            //Debug.Log("Process stopped!");
        }
    }
}
