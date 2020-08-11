﻿using System;
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
using System.Linq;
using System.IO;

public class ScanningInterfaceButton : MonoBehaviour
{
    AudioSource buttonClip;
    InterfaceManager interfaceManager;
    GameObject scanningStart;
    GameObject scanningState;
    GameObject scanningConfirm;
    GameObject scanningIngredientNamesDisplay;
    private List<string> ingredientList = new List<string>();
    private bool doneScanning = false;
    TextMeshPro ingredientNameText1;
    TextMeshPro ingredientNameText2;
    TextMeshPro ingredientNameText3;
    TextMeshPro ingredientNameText4;
    TextMeshPro ingredientNameText5;
    TextMeshPro ingredientNameText6;

    // For image capture
    private bool _isCameraConnected = false;
    private bool _isCapturing = false;
    private bool _hasStarted = false;
    private bool _privilegesBeingRequested = false;
    private Thread _captureThread = null;
    private string currResponse;
    private string url = "http://35.233.198.97:5000/detect_one";
    private object _cameraLockObject = new object();
    private int time_stamp = 0;

    private bool notIn = true;
    
    void Awake()
    {
        scanningStart = GameObject.Find("StartScreen");
        scanningState = GameObject.Find("ScanningText");
        scanningConfirm = GameObject.Find("IngredientScanGet");
        scanningIngredientNamesDisplay = GameObject.Find("Ingredients");

        Debug.Log("nameis: " + GameObject.Find("IngredientName1").GetComponent<TextMeshPro>().ToString());
        ingredientNameText1 = GameObject.Find("IngredientName1").GetComponent<TextMeshPro>();
        ingredientNameText2 = GameObject.Find("IngredientName2").GetComponent<TextMeshPro>();
        ingredientNameText3 = GameObject.Find("IngredientName3").GetComponent<TextMeshPro>();
        ingredientNameText4 = GameObject.Find("IngredientName4").GetComponent<TextMeshPro>();
        ingredientNameText5 = GameObject.Find("IngredientName5").GetComponent<TextMeshPro>();
        ingredientNameText6 = GameObject.Find("IngredientName6").GetComponent<TextMeshPro>();
        ingredientNameText1.text = "";
        ingredientNameText2.text = "";
        ingredientNameText3.text = "";
        ingredientNameText4.text = "";
        ingredientNameText5.text = "";
        ingredientNameText6.text = "";

        interfaceManager = GameObject.Find("InterfaceManager").GetComponent<InterfaceManager>();
        buttonClip = GameObject.Find("Button_Click").GetComponent<AudioSource>();

/*
        Debug.Log("awake");
        if (notIn)
        {
            //test call
            notIn = false;
            testCall("StartScanning");
        }
*/
    }

    public void clicked()
    {
        if (name == "TrashButton 1")
        {
            ingredientList.Remove(ingredientNameText1.text);
            ingredientNameText1.text = "";
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("TrashButton 1").transform.position);
            GameObject.Find("TrashButton 1").SetActive(false);
        } 
        else if (name == "TrashButton 2")
        {
            ingredientList.Remove(ingredientNameText2.text);
            ingredientNameText2.text = "";
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("TrashButton 2").transform.position);
            GameObject.Find("TrashButton 2").SetActive(false);
        }
        else if (name == "TrashButton 3")
        {
            ingredientList.Remove(ingredientNameText3.text);
            ingredientNameText3.text = "";
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("TrashButton 3").transform.position);
            GameObject.Find("TrashButton 3").SetActive(false);
        }
        else if (name == "TrashButton 4")
        {
            ingredientList.Remove(ingredientNameText4.text);
            ingredientNameText4.text = "";
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("TrashButton 4").transform.position);
            GameObject.Find("TrashButton 4").SetActive(false);
        }
        else if (name == "TrashButton 5")
        {
            ingredientList.Remove(ingredientNameText5.text);
            ingredientNameText5.text = "";
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("TrashButton 5").transform.position);
            GameObject.Find("TrashButton 5").SetActive(false);
        }
        else if (name == "TrashButton 6")
        {
            ingredientList.Remove(ingredientNameText6.text);
            ingredientNameText6.text = "";
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("TrashButton 6").transform.position);
            GameObject.Find("TrashButton 6").SetActive(false);
        }
        else if (name == "BackButton")
        {
            scanningConfirm.SetActive(true);
            scanningState.SetActive(false);
            scanningStart.SetActive(false);
            scanningIngredientNamesDisplay.SetActive(false);
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("BackButton").transform.position);
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

            Debug.Log("Started?" + _hasStarted);

            // disable camera now that capture is finished
            if (_isCameraConnected)
            {
                _isCapturing = false;
                DisableMLCamera();
            }

            Debug.Log("Scanning!!");
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("StartScanning").transform.position);
        }
        else if (name == "IngredientList")
        {
            scanningIngredientNamesDisplay.SetActive(true);

            int size = 0;
            if (ingredientList.Count > size)
            {
                ingredientNameText1.text = ingredientList[0];
                size++;
            }
            if (ingredientList.Count > size)
            {
                ingredientNameText2.text = ingredientList[1];
                size++;
            }
            if (ingredientList.Count > size)
            {
                ingredientNameText3.text = ingredientList[2];
                size++;
            }
            if (ingredientList.Count > size)
            {
                ingredientNameText4.text = ingredientList[3];
                size++;
            }
            if (ingredientList.Count > size)
            {
                ingredientNameText5.text = ingredientList[4];
                size++;
            }
            if (ingredientList.Count > size)
            {
                ingredientNameText6.text = ingredientList[5];
                size++;
            }

            if (ingredientNameText1.text.Length < 2)
            {
                GameObject.Find("IngredientNameText 1").SetActive(false);
                GameObject.Find("TrashButton 1").SetActive(false);
            }
            else
            {
                GameObject.Find("IngredientNameText 1").SetActive(true);
                GameObject.Find("TrashButton 1").SetActive(true);
            }

            if (ingredientNameText2.text.Length < 2)
            {
                GameObject.Find("IngredientNameText 2").SetActive(false);
                GameObject.Find("TrashButton 2").SetActive(false);
            }
            else
            {
                GameObject.Find("IngredientNameText 2").SetActive(true);
                GameObject.Find("TrashButton 2").SetActive(true);
            }

            if (ingredientNameText3.text.Length < 2)
            {
                GameObject.Find("IngredientNameText 3").SetActive(false);
                GameObject.Find("TrashButton 3").SetActive(false);
            }
            else
            {
                GameObject.Find("IngredientNameText 3").SetActive(true);
                GameObject.Find("TrashButton 3").SetActive(true);
            }

            if (ingredientNameText4.text.Length < 2)
            {
                GameObject.Find("IngredientNameText 4").SetActive(false);
                GameObject.Find("TrashButton 4").SetActive(false);
            }
            else
            {
                GameObject.Find("IngredientNameText 4").SetActive(true);
                GameObject.Find("TrashButton 4").SetActive(true);
            }

            if (ingredientNameText5.text.Length < 2)
            {
                GameObject.Find("IngredientNameText 5").SetActive(false);
                GameObject.Find("TrashButton 5").SetActive(false);
            }
            else
            {
                GameObject.Find("IngredientNameText 5").SetActive(true);
                GameObject.Find("TrashButton 5").SetActive(true);
            }

            if (ingredientNameText6.text.Length < 2)
            {
                GameObject.Find("IngredientNameText 6").SetActive(false);
                GameObject.Find("TrashButton 6").SetActive(false);
            }
            else
            {
                GameObject.Find("IngredientNameText 6").SetActive(true);
                GameObject.Find("TrashButton 6").SetActive(true);
            }

            scanningState.SetActive(false);
            scanningConfirm.SetActive(false);
            scanningStart.SetActive(false);

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

    //TESTING 
    public void testCall(String name)
    {
        if (name.Equals("TrashButton 1"))
        {
            ingredientList.Remove(ingredientNameText1.text);
            ingredientNameText1.text = "";
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("TrashButton 1").transform.position);
            GameObject.Find("TrashButton 1").SetActive(false);
        }
        else if (name.Equals("TrashButton 2"))
        {
            ingredientList.Remove(ingredientNameText2.text);
            ingredientNameText2.text = "";
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("TrashButton 2").transform.position);
            GameObject.Find("TrashButton 2").SetActive(false);
        }
        else if (name.Equals("TrashButton 3"))
        {
            ingredientList.Remove(ingredientNameText3.text);
            ingredientNameText3.text = "";
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("TrashButton 3").transform.position);
            GameObject.Find("TrashButton 3").SetActive(false);
        }
        else if (name.Equals("TrashButton 4"))
        {
            ingredientList.Remove(ingredientNameText4.text);
            ingredientNameText4.text = "";
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("TrashButton 4").transform.position);
            GameObject.Find("TrashButton 4").SetActive(false);
        }
        else if (name.Equals("TrashButton 5"))
        {
            ingredientList.Remove(ingredientNameText5.text);
            ingredientNameText5.text = "";
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("TrashButton 5").transform.position);
            GameObject.Find("TrashButton 5").SetActive(false);
        }
        else if (name.Equals("TrashButton 6"))
        {
            ingredientList.Remove(ingredientNameText6.text);
            ingredientNameText6.text = "";
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("TrashButton 6").transform.position);
            GameObject.Find("TrashButton 6").SetActive(false);
        }
        else if (name.Equals("GoBack"))
        {
            scanningState.SetActive(false);
            scanningConfirm.SetActive(true);
            scanningStart.SetActive(false);
            scanningIngredientNamesDisplay.SetActive(false);
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("GoBack").transform.position);
        }
        else if (name.Equals("StartScanning"))
        {
            Debug.Log("Begin!!");
            scanningState.SetActive(true);
            scanningConfirm.SetActive(false);
            scanningStart.SetActive(false);
            scanningIngredientNamesDisplay.SetActive(false);

            /*
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

            Debug.Log("Started?" + _hasStarted);

            // disable camera now that capture is finished
            if (_isCameraConnected)
            {
                _isCapturing = false;
                DisableMLCamera();
            }
            */

            doneScanning = true;

            //Debug.Log("Scanning!!");
            //AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("StartScanning").transform.position);
        }
        else if (name.Equals("IngredientList"))
        {
            scanningIngredientNamesDisplay.SetActive(true);

            int size = 0;
            if (ingredientList.Count > size)
            {
                ingredientNameText1.text = ingredientList[0];
                size++;
            }
            if (ingredientList.Count > size)
            {
                ingredientNameText2.text = ingredientList[1];
                size++;
            }
            if (ingredientList.Count > size)
            {
                ingredientNameText3.text = ingredientList[2];
                size++;
            }
            if (ingredientList.Count > size)
            {
                ingredientNameText4.text = ingredientList[3];
                size++;
            }
            if (ingredientList.Count > size)
            {
                ingredientNameText5.text = ingredientList[4];
                size++;
            }
            if (ingredientList.Count > size)
            {
                ingredientNameText6.text = ingredientList[5];
                size++;
            }

            if (ingredientNameText1.text.Length < 2)
            {
                GameObject.Find("IngredientNameText 1").SetActive(false);
                GameObject.Find("TrashButton 1").SetActive(false);
            }
            else
            {
                GameObject.Find("IngredientNameText 1").SetActive(true);
                GameObject.Find("TrashButton 1").SetActive(true);
            }

            if (ingredientNameText2.text.Length < 2)
            {
                GameObject.Find("IngredientNameText 2").SetActive(false);
                GameObject.Find("TrashButton 2").SetActive(false);
            }
            else
            {
                GameObject.Find("IngredientNameText 2").SetActive(true);
                GameObject.Find("TrashButton 2").SetActive(true);
            }

            if (ingredientNameText3.text.Length < 2)
            {
                GameObject.Find("IngredientNameText 3").SetActive(false);
                GameObject.Find("TrashButton 3").SetActive(false);
            }
            else
            {
                GameObject.Find("IngredientNameText 3").SetActive(true);
                GameObject.Find("TrashButton 3").SetActive(true);
            }

            if (ingredientNameText4.text.Length < 2)
            {
                GameObject.Find("IngredientNameText 4").SetActive(false);
                GameObject.Find("TrashButton 4").SetActive(false);
            }
            else
            {
                GameObject.Find("IngredientNameText 4").SetActive(true);
                GameObject.Find("TrashButton 4").SetActive(true);
            }

            if (ingredientNameText5.text.Length < 2)
            {
                GameObject.Find("IngredientNameText 5").SetActive(false);
                GameObject.Find("TrashButton 5").SetActive(false);
            }
            else
            {
                GameObject.Find("IngredientNameText 5").SetActive(true);
                GameObject.Find("TrashButton 5").SetActive(true);
            }

            if (ingredientNameText6.text.Length < 2)
            {
                GameObject.Find("IngredientNameText 6").SetActive(false);
                GameObject.Find("TrashButton 6").SetActive(false);
            }
            else
            {
                GameObject.Find("IngredientNameText 6").SetActive(true);
                GameObject.Find("TrashButton 6").SetActive(true);
            }

            scanningState.SetActive(false);
            scanningConfirm.SetActive(false);
            scanningStart.SetActive(false);
            
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("IngredientList").transform.position);
        }
        else if (name.Equals("GetRecipes"))
        {
            // !! THIS WILL GO TO THE NEXT SCREEN WITH RECIPES
            // use IngredientList to access current list of ingredients
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("GetRecipes").transform.position);
        }
        else if (name.Equals("ScanMoreItems"))
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
        if (doneScanning)
        {
            scanningConfirm.SetActive(true);

            /*
            string response = "{\"detections\": " + currResponse + " }";
            ListOfDetections listOfDetections;

            try
            {
                listOfDetections = JsonUtility.FromJson<ListOfDetections>(response);
            }
            catch (Exception e)
            {
                return;
            }

            for (int i = 0; listOfDetections.detections.Count > i; i++)
            {
                ingredientList.Add(listOfDetections.detections[i].label);
            }
            */

            List<String> test = new List<String>() { "cheese", "cheese", "eggs", "milk" };
            for (int i = 0; test.Count > i; i++)
            {
                ingredientList.Add(test[i]);
            }

            // remove any duplicates
            ingredientList = ingredientList.Distinct().ToList();

            doneScanning = false;
            testCall("IngredientList");
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

    private async void UploadFile(string uri, byte[] rawImage, int stamp)
    {
        Debug.Log("Uploading...");

        WebClient myWebClient = new WebClient();
        myWebClient.Headers.Add("Content-Type", "binary/octet-stream");
        myWebClient.Encoding = Encoding.UTF8;
        Debug.Log("Sending...");
        byte[] responseArray = await myWebClient.UploadDataTaskAsync(uri, rawImage);  // send a byte array to the resource and returns a byte array containing any response
        Debug.Log("Sent!");
        currResponse = Encoding.UTF8.GetString(responseArray);
        doneScanning = true;
        Debug.Log("Currrent Response:" + currResponse);
    }


    private void OnCaptureRawImageComplete(byte[] imageData)
    {
        lock (_cameraLockObject)
        {
            _isCapturing = false;
        }
        Debug.Log("capture complete");

        time_stamp++;
        UploadFile(url, imageData, time_stamp - 1);
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

    [Serializable]
    public class ListOfDetections
    {
        public List<Det> detections;
    }

    [Serializable]
    public class Det
    {
        public List<int> boxes;
        public string label;
        public float confidence;
    }
}