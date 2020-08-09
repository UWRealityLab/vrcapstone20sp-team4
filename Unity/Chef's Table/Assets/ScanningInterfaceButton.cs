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
    AudioSource buttonClip;
    InterfaceManager interfaceManager;
    GameObject scanningStart;
    GameObject scanningState;
    GameObject scanningConfirm;
    GameObject scanningIngredientNamesDisplay;
    private List<string> ingredientList = new List<string>();
    private bool isScanning = false;
    Text ingredientNameText1;
    Text ingredientNameText2;
    Text ingredientNameText3;
    Text ingredientNameText4;
    Text ingredientNameText5;
    Text ingredientNameText6;

    // For image capture
    private bool _isCameraConnected = false;
    private bool _isCapturing = false;
    private bool _hasStarted = false;
    private bool _privilegesBeingRequested = false;
    private Thread _captureThread = null;
    private float timer = 5;
    private string currResponse;
    public TextMeshProUGUI UI;
    private string url = "http://35.233.198.97";
    private object _cameraLockObject = new object();

    void Awake()
    {
        scanningStart = GameObject.Find("ScanningStart");
        scanningState = GameObject.Find("ScanningState");
        scanningConfirm = GameObject.Find("ScanningConfirm");
        scanningIngredientNamesDisplay = GameObject.Find("ScanningIngredientNamesDisplay");

        ingredientNameText1 = GameObject.Find("ingredientNameText1").GetComponent<Text>();
        ingredientNameText2 = GameObject.Find("ingredientNameText2").GetComponent<Text>();
        ingredientNameText3 = GameObject.Find("ingredientNameText3").GetComponent<Text>();
        ingredientNameText4 = GameObject.Find("ingredientNameText4").GetComponent<Text>();
        ingredientNameText5 = GameObject.Find("ingredientNameText5").GetComponent<Text>();
        ingredientNameText6 = GameObject.Find("ingredientNameText6").GetComponent<Text>();

        interfaceManager = GameObject.Find("InterfaceManager").GetComponent<InterfaceManager>();
        buttonClip = GameObject.Find("Button_Click").GetComponent<AudioSource>();
        //GameObject iconText = transform.parent.transform.Find("IconAndText").gameObject;
        //icon = iconText.transform.Find("Icon").gameObject;
        //NIControl = GameObject.Find("HeadLockCanvas").GetComponent<NIThresholdControl>();
    }

    public void clicked()
    {
        if (name == "TrashButton 1")
        {
            ingredientList.Remove(ingredientNameText1.text);
            ingredientNameText1.text = "";
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("TrashButton 1").transform.position);
        } 
        else if (name == "TrashButton 2")
        {
            ingredientList.Remove(ingredientNameText2.text);
            ingredientNameText2.text = "";
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("TrashButton 2").transform.position);
        }
        else if (name == "TrashButton 3")
        {
            ingredientList.Remove(ingredientNameText3.text);
            ingredientNameText3.text = "";
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("TrashButton 3").transform.position);
        }
        else if (name == "TrashButton 4")
        {
            ingredientList.Remove(ingredientNameText4.text);
            ingredientNameText4.text = "";
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("TrashButton 4").transform.position);
        }
        else if (name == "TrashButton 5")
        {
            ingredientList.Remove(ingredientNameText5.text);
            ingredientNameText5.text = "";
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("TrashButton 5").transform.position);
        }
        else if (name == "TrashButton 6")
        {
            ingredientList.Remove(ingredientNameText6.text);
            ingredientNameText6.text = "";
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
            scanningState.SetActive(true);
            scanningConfirm.SetActive(false);
            scanningStart.SetActive(false);
            scanningIngredientNamesDisplay.SetActive(false);

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

            isScanning = true;
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("StartScanning").transform.position);
        }
        else if (name == "IngredientList")
        {
            scanningState.SetActive(false);
            scanningConfirm.SetActive(false);
            scanningStart.SetActive(false);
            scanningIngredientNamesDisplay.SetActive(true);

            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("IngredientList").transform.position);
        }
        else if (name == "GetRecipes")
        {
            // !! THIS WILL GO TO THE NEXT SCREEN WITH RECIPES 
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("GetRecipes").transform.position);
        }
        else if (name == "ScanMoreItems")
        {
            scanningState.SetActive(true);
            scanningConfirm.SetActive(false);
            scanningStart.SetActive(false);
            scanningIngredientNamesDisplay.SetActive(false);

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
            // ingredientNameText1.text ........
            // ingredientNameText2.text ........
            // ingredientNameText3.text ........
            // ingredientNameText4.text ........
            // ingredientNameText5.text ........
            // ingredientNameText6.text ........

            // ingredientList = currResponse;
            scanningConfirm.SetActive(false);
            scanningState.SetActive(true);
            scanningStart.SetActive(false);
            scanningIngredientNamesDisplay.SetActive(false);
            isScanning = false;
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