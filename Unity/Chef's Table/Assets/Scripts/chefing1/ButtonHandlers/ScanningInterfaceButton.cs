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
    GameObject ingredientNameText1Display;
    GameObject ingredientNameText2Display;
    GameObject ingredientNameText3Display;
    GameObject ingredientNameText4Display;
    GameObject ingredientNameText5Display;
    GameObject ingredientNameText6Display;
    GameObject trashButton1;
    GameObject trashButton2;
    GameObject trashButton3;
    GameObject trashButton4;
    GameObject trashButton5;
    GameObject trashButton6;
    private string ingredientListString = "";
    private bool doneScanning = false;
    TextMeshPro ingredientNameText1;
    TextMeshPro ingredientNameText2;
    TextMeshPro ingredientNameText3;
    TextMeshPro ingredientNameText4;
    TextMeshPro ingredientNameText5;
    TextMeshPro ingredientNameText6;
    private float timer = 2;

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

    private float sendIngredientsdelayTime = 2.0f;
    private GetInstructions recipeApi;

    private IngredientListScript ingredientList;
    void Awake()
    {
        scanningStart = GameObject.Find("StartScreen");
        scanningState = GameObject.Find("ScanningText");
        scanningConfirm = GameObject.Find("IngredientScanGet");
        scanningIngredientNamesDisplay = GameObject.Find("Ingredients");

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

        ingredientNameText1Display = GameObject.Find("IngredientNameText 1");
        ingredientNameText2Display = GameObject.Find("IngredientNameText 2");
        ingredientNameText3Display = GameObject.Find("IngredientNameText 3");
        ingredientNameText4Display = GameObject.Find("IngredientNameText 4");
        ingredientNameText5Display = GameObject.Find("IngredientNameText 5");
        ingredientNameText6Display = GameObject.Find("IngredientNameText 6");

        trashButton1 = GameObject.Find("TrashButton 1");
        trashButton2 = GameObject.Find("TrashButton 2");
        trashButton3 = GameObject.Find("TrashButton 3");
        trashButton4 = GameObject.Find("TrashButton 4");
        trashButton5 = GameObject.Find("TrashButton 5");
        trashButton6 = GameObject.Find("TrashButton 6");

        interfaceManager = GameObject.Find("InterfaceManager").GetComponent<InterfaceManager>();
        buttonClip = GameObject.Find("Button_Click").GetComponent<AudioSource>();
        recipeApi = GameObject.Find("RecipeAPI").GetComponent<GetInstructions>();
        ingredientList = GameObject.Find("IngredientList").GetComponent<IngredientListScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hand")
        {
            GetComponent<Button>().onClick.Invoke();
        }
    }

    public void clicked()
    {
        if (name == "TrashButtonScript 1")
        {
            ingredientList.removeFromList(ingredientNameText1.text);
            ingredientNameText1.text = "";
            AudioSource.PlayClipAtPoint(buttonClip.clip, trashButton1.transform.position);
            trashButton1.SetActive(false);
        }
        else if (name == "TrashButtonScript 2")
        {
            ingredientList.removeFromList(ingredientNameText2.text);
            ingredientNameText2.text = "";
            AudioSource.PlayClipAtPoint(buttonClip.clip, trashButton2.transform.position);
            trashButton2.SetActive(false);
        }
        else if (name == "TrashButtonScript 3")
        {
            ingredientList.removeFromList(ingredientNameText3.text);
            ingredientNameText3.text = "";
            AudioSource.PlayClipAtPoint(buttonClip.clip, trashButton3.transform.position);
            trashButton3.SetActive(false);
        }
        else if (name == "TrashButtonScript 4")
        {
            ingredientList.removeFromList(ingredientNameText4.text);
            ingredientNameText4.text = "";
            AudioSource.PlayClipAtPoint(buttonClip.clip, trashButton4.transform.position);
            trashButton4.SetActive(false);
        }
        else if (name == "TrashButtonScript 5")
        {
            ingredientList.removeFromList(ingredientNameText5.text);
            ingredientNameText5.text = "";
            AudioSource.PlayClipAtPoint(buttonClip.clip, trashButton5.transform.position);
            trashButton5.SetActive(false);
        }
        else if (name == "TrashButtonScript 6")
        {
            ingredientList.removeFromList(ingredientNameText6.text);
            ingredientNameText6.text = "";
            AudioSource.PlayClipAtPoint(buttonClip.clip, trashButton6.transform.position);
            trashButton6.SetActive(false);
        }
        else if (name == "BackButtonScript")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("BackButtonScript").transform.position);
            scanningConfirm.SetActive(true);
            scanningState.SetActive(false);
            scanningStart.SetActive(false);
            scanningIngredientNamesDisplay.SetActive(false);
        }
        else if (name == "StartScanningButton")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("StartScanningButton").transform.position);
            scanningState.SetActive(true);
            scanningConfirm.SetActive(false);
            scanningStart.SetActive(false);
            scanningIngredientNamesDisplay.SetActive(false);

            // two second timer so that the scene state can change
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                timer = 2;
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
            }
        }
        else if (name == "IngredientListButton")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("IngredientListButton").transform.position);

            // change the displayed text
            int size = 0;
            if (ingredientList.count() > size)
            {
                ingredientNameText1.text = ingredientList.get(0);
                size++;
            }
            if (ingredientList.count() > size)
            {
                ingredientNameText2.text = ingredientList.get(1);
                size++;
            }
            if (ingredientList.count() > size)
            {
                ingredientNameText3.text = ingredientList.get(2);
                size++;
            }
            if (ingredientList.count() > size)
            {
                ingredientNameText4.text = ingredientList.get(3);
                size++;
            }
            if (ingredientList.count() > size)
            {
                ingredientNameText5.text = ingredientList.get(4);
                size++;
            }
            if (ingredientList.count() > size)
            {
                ingredientNameText6.text = ingredientList.get(5);
            }

            if (ingredientNameText1.text.Length > 1)
            {
                ingredientNameText1Display.SetActive(true);
                trashButton1.SetActive(true);
            }
            else
            {
                ingredientNameText1Display.SetActive(false);
                trashButton1.SetActive(false);
            }

            if (ingredientNameText2.text.Length > 1)
            {
                ingredientNameText2Display.SetActive(true);
                trashButton2.SetActive(true);
            }
            else
            {
                ingredientNameText2Display.SetActive(false);
                trashButton2.SetActive(false);
            }

            if (ingredientNameText3.text.Length > 1)
            {
                ingredientNameText3Display.SetActive(true);
                trashButton3.SetActive(true);
            }
            else
            {
                ingredientNameText3Display.SetActive(false);
                trashButton3.SetActive(false);
            }

            if (ingredientNameText4.text.Length > 1)
            {
                ingredientNameText4Display.SetActive(true);
                trashButton4.SetActive(true);
            }
            else
            {
                ingredientNameText4Display.SetActive(false);
                trashButton4.SetActive(false);
            }

            if (ingredientNameText5.text.Length > 1)
            {
                ingredientNameText5Display.SetActive(true);
                trashButton5.SetActive(true);
            }
            else
            {
                ingredientNameText5Display.SetActive(false);
                trashButton5.SetActive(false);
            }

            if (ingredientNameText6.text.Length > 1)
            {
                ingredientNameText6Display.SetActive(true);
                trashButton6.SetActive(true);
            }
            else
            {
                ingredientNameText6Display.SetActive(false);
                trashButton6.SetActive(false);
            }

            scanningIngredientNamesDisplay.SetActive(true);
            scanningState.SetActive(false);
            scanningConfirm.SetActive(false);
            scanningStart.SetActive(false);
        }
        else if (name == "GetRecipesButton")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("GetRecipesButton").transform.position);
            ingredientListString = string.Join(",", ingredientList.array());
            recipeApi.GetIngredientsList(ingredientListString);
            interfaceManager.setActiveOnboardingInterface(true);
            interfaceManager.setActiveScanningInterface(false);
        }
        else if (name == "ScanMoreItemsButton")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("ScanMoreItemsButton").transform.position);
            scanningState.SetActive(false);
            scanningConfirm.SetActive(false);
            scanningStart.SetActive(true);
            scanningIngredientNamesDisplay.SetActive(false);
        }
        else
        {
            Debug.Log("Unknown button");
        }
    }

    /*
    public string getIngredients()
    {
        sendIngredientsdelayTime -= 0.5f;
        if (sendIngredientsdelayTime <= 0) {
            ingredientListString = "avocado";
        }
        return ingredientListString;
    }
    */

    private void updateScanningInterface()
    {
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
            ingredientList.addToList(listOfDetections.detections[i].label);
        }*/

        
        List<String> test = new List<String>() { "cheese", "cheese", "eggs", "milk" };
        for (int i = 0; test.Count > i; i++)
        {
            ingredientList.addToList(test[i]);
        }

        // remove any duplicates
        ingredientList.distinct();

        scanningState.SetActive(false);
        scanningConfirm.SetActive(true);
        scanningStart.SetActive(false);
        scanningIngredientNamesDisplay.SetActive(false);
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

        WebClient myWebClient = new WebClient();
        myWebClient.Headers.Add("Content-Type", "binary/octet-stream");
        myWebClient.Encoding = Encoding.UTF8;
        //byte[] responseArray = await myWebClient.UploadDataTaskAsync(uri, rawImage);  // send a byte array to the resource and returns a byte array containing any response
        //currResponse = Encoding.UTF8.GetString(responseArray);
        updateScanningInterface();
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