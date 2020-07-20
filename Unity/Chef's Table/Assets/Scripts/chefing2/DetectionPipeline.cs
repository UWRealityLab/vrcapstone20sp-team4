using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.XR.MagicLeap;
using System.Collections.Generic;
using System.Threading;
using MagicLeap.Core.StarterKit;
using System.Net;
using System.Text;
using TMPro;
using System.Security.Policy;

public class DetectionPipeline : MonoBehaviour
{

    public GameObject _previewObject;

    private bool _isCameraConnected = false;

    private bool _isCapturing = false;

    private bool _hasStarted = false;

    private bool _privilegesBeingRequested = false;

    private Thread _captureThread = null;

    private float timer = 5;

    private string currResponse;
    public TextMeshProUGUI UI;

    private string url = "http://35.225.232.30:5000/predict";
    /// <summary>
    /// The example is using threads on the call to MLCamera.CaptureRawImageAsync to alleviate the blocking
    /// call at the beginning of CaptureRawImageAsync, and the safest way to prevent race conditions here is to
    /// lock our access into the MLCamera class, so that we don't accidentally shut down the camera
    /// while the thread is attempting to work
    /// </summary>
    private object _cameraLockObject = new object();


    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            
            timer = 5;
            TriggerAsyncCapture();
        }
        UI.text = currResponse;


    }

    /// <summary>
    /// Using Awake so that Privileges is set before PrivilegeRequester Start.
    /// </summary>
    void Awake()
    {


        if (_previewObject == null)
        {
            Debug.LogError("Error: ImageCaptureExample._previewObject is not set, disabling script.");
            enabled = false;
            return;
        }

        // This is made active when we have a captured image to show.
        _previewObject.SetActive(false);

        // Before enabling the Camera, the scene must wait until the privilege has been granted.
        MLResult result = MLPrivilegesStarterKit.Start();
#if PLATFORM_LUMIN
        if (!result.IsOk)
        {
            Debug.LogErrorFormat("Error: ImageCaptureExample failed starting MLPrivilegesStarterKit, disabling script. Reason: {0}", result);
            enabled = false;
            return;
        }
#endif

        result = MLPrivilegesStarterKit.RequestPrivilegesAsync(HandlePrivilegesDone, MLPrivileges.Id.CameraCapture);
#if PLATFORM_LUMIN
        if (!result.IsOk)
        {
            Debug.LogErrorFormat("Error: ImageCaptureExample failed requesting privileges, disabling script. Reason: {0}", result);
            MLPrivilegesStarterKit.Stop();
            enabled = false;
            return;
        }
#endif

        _privilegesBeingRequested = true;
    }

    /// <summary>
    /// Stop the camera, unregister callbacks, and stop input and privileges APIs.
    /// </summary>
    void OnDisable()
    {
#if PLATFORM_LUMIN
        MLInput.OnControllerButtonDown -= OnButtonDown;
#endif

        lock (_cameraLockObject)
        {
            if (_isCameraConnected)
            {
#if PLATFORM_LUMIN
                MLCamera.OnRawImageAvailable -= OnCaptureRawImageComplete;
#endif

                _isCapturing = false;
                DisableMLCamera();
            }
        }
    }

    /// <summary>
    /// Cannot make the assumption that a reality privilege is still granted after
    /// returning from pause. Return the application to the state where it
    /// requests privileges needed and clear out the list of already granted
    /// privileges. Also, disable the camera and unregister callbacks.
    /// </summary>
    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            lock (_cameraLockObject)
            {
                if (_isCameraConnected)
                {
#if PLATFORM_LUMIN
                    MLCamera.OnRawImageAvailable -= OnCaptureRawImageComplete;
#endif

                    _isCapturing = false;

                    DisableMLCamera();
                }
            }

#if PLATFORM_LUMIN
            MLInput.OnControllerButtonDown -= OnButtonDown;
#endif

            _hasStarted = false;
        }
    }

    /// <summary>
    /// Cleans up the component.
    /// </summary>
    void OnDestroy()
    {
        if (_privilegesBeingRequested)
        {
            _privilegesBeingRequested = false;

            MLPrivilegesStarterKit.Stop();
        }
    }


    /// <summary>
    /// Captures a still image using the device's camera and returns
    /// the data path where it is saved.
    /// </summary>
    /// <param name="fileName">The name of the file to be saved to.</param>
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

    /// <summary>
    /// Connects the MLCamera component and instantiates a new instance
    /// if it was never created.
    /// </summary>
    private void EnableMLCamera()
    {
#if PLATFORM_LUMIN
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
#endif
    }

    /// <summary>
    /// Disconnects the MLCamera if it was ever created or connected.
    /// </summary>
    private void DisableMLCamera()
    {
#if PLATFORM_LUMIN
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
#endif
    }

    /// <summary>
    /// Once privileges have been granted, enable the camera and callbacks.
    /// </summary>
    private void StartCapture()
    {
        if (!_hasStarted)
        {
            lock (_cameraLockObject)
            {
                EnableMLCamera();

#if PLATFORM_LUMIN
                MLCamera.OnRawImageAvailable += OnCaptureRawImageComplete;
#endif
            }

#if PLATFORM_LUMIN
            MLInput.OnControllerButtonDown += OnButtonDown;
#endif

            _hasStarted = true;
        }
    }

    /// <summary>
    /// Responds to privilege requester result.
    /// </summary>
    /// <param name="result"/>
    private void HandlePrivilegesDone(MLResult result)
    {
        _privilegesBeingRequested = false;
        MLPrivilegesStarterKit.Stop();

#if PLATFORM_LUMIN
        if (result != MLResult.Code.PrivilegeGranted)
        {
            Debug.LogErrorFormat("Error: ImageCaptureExample failed to get requested privileges, disabling script. Reason: {0}", result);
            enabled = false;
            return;
        }
#endif

        Debug.Log("Succeeded in requesting all privileges");
        StartCapture();
    }

    /// <summary>
    /// Handles the event for button down.
    /// </summary>
    /// <param name="controllerId">The id of the controller.</param>
    /// <param name="button">The button that is being pressed.</param>
    private void OnButtonDown(byte controllerId, MLInput.Controller.Button button)
    {
        TriggerAsyncCapture();
    }

    /// <summary>
    /// Handles the event of a new image getting captured.
    /// </summary>
    /// <param name="imageData">The raw data of the image.</param>
    private void OnCaptureRawImageComplete(byte[] imageData)
    {
        lock (_cameraLockObject)
        {
            _isCapturing = false;
        }
        // Initialize to 8x8 texture so there is no discrepency
        // between uninitalized captures and error texture
        Texture2D texture = new Texture2D(8, 8);
        bool status = texture.LoadImage(imageData);

        if (status && (texture.width != 8 && texture.height != 8))
        {
            _previewObject.SetActive(true);
            Renderer renderer = _previewObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.mainTexture = texture;
            }
        }
        UploadFile(url, imageData);
    }

    /// <summary>
    /// Worker function to call the API's Capture function
    /// </summary>
    private void CaptureThreadWorker()
    {
#if PLATFORM_LUMIN
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
#endif
    }

    private async void UploadFile(string uri, byte[] rawImage)
    {
        WebClient myWebClient = new WebClient();
        myWebClient.Headers.Add("Content-Type", "binary/octet-stream");
        myWebClient.Encoding = Encoding.UTF8;
        byte[] responseArray = await myWebClient.UploadDataTaskAsync(uri, rawImage); ;  // send a byte array to the resource and returns a byte array containing any response
        currResponse = Encoding.UTF8.GetString(responseArray);
    }
}

