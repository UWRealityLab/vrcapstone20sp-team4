using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading;
using MagicLeap.Core.StarterKit;
using System.Net;
using System.Text;
using System.Security.Policy;
using UnityEngine.XR.MagicLeap;
using UnityEngine.Events;

public class ScanningInterfaceButton : MonoBehaviour
{
    // Start is called before the first frame update
    MainScheduler scheduler;
    AudioSource buttonClip;
    AudioSource timerClip;
    GameObject text;
    GameObject icon;
    NIThresholdControl NIControl;
    InterfaceManager interfaceManager;
    GameObject scanningStart;
    GameObject scanningState;
    GameObject scanningConfirm;
    GameObject scanningIngredientNamesDisplay;
    private string[] ingredientList;
    private bool isScanning = false;
    
    // For image capture
    private bool _isCameraConnected = false;
    private bool _isCapturing = false;
    private bool _hasStarted = false;
    private bool _privilegesBeingRequested = false;
    private Thread _captureThread = null;
    private float timer = 5;
    private string currResponse;
    public TextMeshProUGUI UI;
    private string url = "http://35.225.232.30:5000/predict";
    private object _cameraLockObject = new object();

    void Awake()
    {
        scanningStart = GameObject.Find("ScanningStart");
        scanningState = GameObject.Find("ScanningState");
        scanningConfirm = GameObject.Find("ScanningConfirm");
        scanningIngredientNamesDisplay = GameObject.Find("ScanningIngredientNamesDisplay");

        scheduler = GameObject.Find("Scheduler").GetComponent<MainScheduler>();
        interfaceManager = GameObject.Find("InterfaceManager").GetComponent<InterfaceManager>();
        buttonClip = GameObject.Find("Button_Click").GetComponent<AudioSource>();
        timerClip = GameObject.Find("Timer").GetComponent<AudioSource>();
        GameObject iconText = transform.parent.transform.Find("IconAndText").gameObject;
        icon = iconText.transform.Find("Icon").gameObject;
        NIControl = GameObject.Find("HeadLockCanvas").GetComponent<NIThresholdControl>();
        //pauseButton = Resources.Load("Mat/ButtonPauseMat", typeof(Material)) as Material;
    }

    public void clicked()
    {
        if (name == "TrashButton 1")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("TrashButton 1").transform.position);
        }
        else if (name == "TrashButton 2")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("TrashButton 2").transform.position);
        }
        else if (name == "TrashButton 3")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("TrashButton 3").transform.position);
        }
        else if (name == "TrashButton 4")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("TrashButton 4").transform.position);
        }
        else if (name == "TrashButton 5")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("TrashButton 5").transform.position);
        }
        else if (name == "TrashButton 6")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("TrashButton 6").transform.position);
        }
        else if (name == "GoBack")
        {
            // scanningConfirm.SetActive(true);
            // set everything else to false
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("GoBack").transform.position);
        }
        else if (name == "StartScanning")
        {
            
            // Before enabling the Camera, the scene must wait until the privilege has been granted.
            MLResult result = MLPrivilegesStarterKit.Start();

            if (!result.IsOk)
            {
                Debug.LogErrorFormat("Error: ScanningInterfaceButton failed starting MLPrivilegesStarterKit, disabling script. Reason: {0}", result);
                enabled = false;
                return;
            }
            result = MLPrivilegesStarterKit.RequestPrivilegesAsync(HandlePrivilegesDone, MLPrivileges.Id.CameraCapture);

            if (!result.IsOk)
            {
                Debug.LogErrorFormat("Error: ScanningInterfaceButton failed requesting privileges, disabling script. Reason: {0}", result);
                MLPrivilegesStarterKit.Stop();
                enabled = false;
                return;
            }

            _privilegesBeingRequested = true;

            // disable camera now that capture is finished
            if (_isCameraConnected)
            {
                _isCapturing = false;
                DisableMLCamera();
            }

            // scanningState.SetActive(true);
            isScanning = true;

            // everything else set false
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("StartScanning").transform.position);
        }
        else if (name == "IngredientList")
        {
            // scanningIngredientNamesDisplay.SetActive(true);
            // everything else set false
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("IngredientList").transform.position);
        }
        else if (name == "GetRecipes")
        {
            // !! THIS WILL GO TO THE NEXT SCREEN WITH RECIPES 
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("GetRecipes").transform.position);
        }
        else if (name == "ScanMoreItems")
        {
            // scanningState.SetActive(true);
            // everything else set false
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("ScanMoreItems").transform.position);
        }
        else
        {
            Debug.Log("Unknown button");
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = 5;
            // scanningConfirm.SetActive(true);
            // set everything else to false
            // isScanning = false;
            // ingredientList = currResponse;
        }
    }

    public void TriggerAsyncCapture()
    {
        if (_captureThread == null || (!_captureThread.IsAlive))
        {
            ThreadStart captureThreadStart = new ThreadStart(CaptureThreadWorker);
            _captureThread = new Thread(captureThreadStart);
            _captureThread.Start();
        }
        else
        {
            Debug.Log("Previous thread has not finished, unable to begin a new capture just yet.");
        }
    }

    private void EnableMLCamera()
    {
        lock (_cameraLockObject)
        {
            MLResult result = MLCamera.Start();
            if (result.IsOk)
            {
                result = MLCamera.Connect();
                _isCameraConnected = true;
            }
            else
            {
                Debug.LogErrorFormat("Error: ImageCaptureExample failed starting MLCamera, disabling script. Reason: {0}", result);
                enabled = false;
                return;
            }
        }
    }

    private void StartCapture()
    {
        if (!_hasStarted)
        {
            lock (_cameraLockObject)
            {
                EnableMLCamera();
                MLCamera.OnRawImageAvailable += OnCaptureRawImageComplete;
            }
            TriggerAsyncCapture();
            _hasStarted = true;
        }
    }

    private void HandlePrivilegesDone(MLResult result)
    {
        _privilegesBeingRequested = false;
        MLPrivilegesStarterKit.Stop();
        if (result != MLResult.Code.PrivilegeGranted)
        {
            Debug.LogErrorFormat("Error: ImageCaptureExample failed to get requested privileges, disabling script. Reason: {0}", result);
            enabled = false;
            return;
        }
        Debug.Log("Succeeded in requesting all privileges");
        StartCapture();
    }

    private async void UploadFile(string uri, byte[] rawImage)
    {
        WebClient myWebClient = new WebClient();
        myWebClient.Headers.Add("Content-Type", "binary/octet-stream");
        myWebClient.Encoding = Encoding.UTF8;
        byte[] responseArray = await myWebClient.UploadDataTaskAsync(uri, rawImage); ;  // send a byte array to the resource and returns a byte array containing any response
        currResponse = Encoding.UTF8.GetString(responseArray);
    }

    private void OnCaptureRawImageComplete(byte[] imageData)
    {
        UploadFile(url, imageData);
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            lock (_cameraLockObject)
            {
                if (_isCameraConnected)
                {
                    MLCamera.OnRawImageAvailable -= OnCaptureRawImageComplete;
                    _isCapturing = false;

                    DisableMLCamera();
                }
            }

            _hasStarted = false;
        }
    }

    void OnDestroy()
    {
        if (_privilegesBeingRequested)
        {
            _privilegesBeingRequested = false;

            MLPrivilegesStarterKit.Stop();
        }
    }

    void OnDisable()
    {
        lock (_cameraLockObject)
        {
            if (_isCameraConnected)
            {
                MLCamera.OnRawImageAvailable -= OnCaptureRawImageComplete;

                _isCapturing = false;
                DisableMLCamera();
            }
        }
    }
    private void DisableMLCamera()
    {
        lock (_cameraLockObject)
        {
            if (MLCamera.IsStarted)
            {
                MLCamera.Disconnect();
                // Explicitly set to false here as the disconnect was attempted.
                _isCameraConnected = false;
                MLCamera.Stop();
            }
        }
    }
    private void CaptureThreadWorker()
    {
        lock (_cameraLockObject)
        {
            if (MLCamera.IsStarted && _isCameraConnected)
            {
                MLResult result = MLCamera.CaptureRawImageAsync();
                if (result.IsOk)
                {
                    _isCapturing = true;
                }
            }
        }
    }
}