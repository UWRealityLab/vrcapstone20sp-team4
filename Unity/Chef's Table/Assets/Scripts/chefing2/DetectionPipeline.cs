using System;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using System.Collections.Generic;
using System.Threading;
using MagicLeap.Core.StarterKit;
using System.Net;
using System.Text;
using System.Linq;

public class DetectionPipeline : MonoBehaviour
{

    private bool _isCameraConnected = false;

    private bool _isCapturing = false;

    private bool _hasStarted = false;

    private bool _privilegesBeingRequested = false;

    private Thread _captureThread = null;

    private float timer = 3;

    private string currResponse = "";
    private int time_stamp = 0;
    private float Cx = 949.5f;
    private float Cy = 539.1f;
    private float Fx = 1400f;
    private float Fy = 1399.5f;
    public Transform ctransform;
    private string url_tutorial = "http://35.233.198.97:5000/predict";
    private string url_suggest = "http://35.233.198.97:5000/detect_one";
    private Raycast rc;
    public GameObject copy_prefab;
    private Dictionary<int, GameObject> stamp2Copy = new Dictionary<int, GameObject>();
    private object _cameraLockObject = new object();
    private MainScheduler2 mainScheduler;
    private bool makingSuggestion = true;  // true for recipe suggestion, false for tutorial
    public GameObject scanningInterfaceContainer;
    private bool STARTCAPTURE = false;
    private void Start()
    {


        rc = GameObject.Find("RaycastNode").GetComponent<Raycast>();
        mainScheduler = GameObject.Find("MainScheduler").GetComponent<MainScheduler2>();

    }

    void Update()
    {
        if (STARTCAPTURE)
        {
            timer -= Time.deltaTime;

            if (timer < 0)
            {
                // stamp2Copy[time_stamp] = Instantiate(copy_prefab, ctransform.position, ctransform.rotation);
                timer = 3;
                TriggerAsyncCapture();
                if (!makingSuggestion)
                {
                    findDetectedObjects();
                }
                
            }

        } else
        {
            timer = 3;
        }

    }

    public void startPipeline(bool suggestionMode)
    {
        this.makingSuggestion = suggestionMode;
        this.STARTCAPTURE = true;
    }

    public void stopPipeline()
    {
        this.makingSuggestion = false;
        this.STARTCAPTURE = false;
    }
    private void resetState()
    {
        timer = 3;
        time_stamp = 0;
        stamp2Copy.Clear();
        currResponse = "";
        stopPipeline();
    }

    // parses detection result and find their location in by raycasting
    public void findDetectedObjects()
    {
        string response = "{\"detections\": " + currResponse + " }";
        DetectionList DL;
        try
        {
            DL = JsonUtility.FromJson<DetectionList>(response);
        }
        catch (Exception e)
        {
            return;
        }
        Dictionary<string, Vector3> rays = new Dictionary<string, Vector3>();
        int stamp = DL.detections[0].stamp;
        if (!stamp2Copy.ContainsKey(stamp)) return;
        foreach (var detection in DL.detections)
        {
            int x1, y1, x2, y2;
            x1 = detection.boxes[0];
            y1 = detection.boxes[1];
            x2 = detection.boxes[2];
            y2 = detection.boxes[3];
            if (x1 < 0 || y1 < 0 || x2 > 1920 || y2 > 1080)
            {
                continue;
            }
            Vector3 center = getRaycastPointWorldSpace((x1 + x2) / 2f, 1080 - ((y1 + y2) / 2f), stamp);
            rays[detection.label] = center;
        }
        rc.setCpoition(stamp2Copy[stamp].transform.position);
        Destroy(stamp2Copy[stamp]);
        stamp2Copy.Remove(stamp);
        rc.makeRayCast2(rays, true);

    }

    // calculate reference point to do raycast from a point on the image
    public Vector3 getRaycastPointWorldSpace(float u, float v, int time_stamp)
    {

        float z = 1;
        float x = (u - Cx) / Fx;
        float y = (v - Cy) / Fy;
        return stamp2Copy[time_stamp].transform.TransformPoint(new Vector3(x, y, z));
    }

    private async void UploadFile(byte[] rawImage, int stamp)
    {
        Debug.Log("sending to cloud");
        WebClient myWebClient = new WebClient();
        myWebClient.Headers.Add("Content-Type", "binary/octet-stream");
        myWebClient.Encoding = Encoding.UTF8;
        if (makingSuggestion)
        {
            try
            {
                byte[] responseArray = await myWebClient.UploadDataTaskAsync(url_suggest, rawImage);
                currResponse = Encoding.UTF8.GetString(responseArray);
                scanningInterfaceContainer.GetComponent<ScanningInterfaceController>().updateIngredientList(currResponse);
            } catch (Exception e)
            {
                scanningInterfaceContainer.GetComponent<ScanningInterfaceController>().updateIngredientList("error");
            }
            
        }
        else
        {
            byte[] timeByte = BitConverter.GetBytes(stamp);
            byte[] imageInfo = rawImage.Concat(timeByte).ToArray();
            byte[] responseArray = await myWebClient.UploadDataTaskAsync(url_tutorial, imageInfo);
            currResponse = Encoding.UTF8.GetString(responseArray);
        }

    }


    [Serializable]
    public class DetectionList
    {
        public List<Detection> detections;
    }

    [Serializable]
    public class Detection
    {
        public int stamp;
        public List<int> boxes;
        public string label;
        public float confidence;
    }

    void Awake()
    {
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


    void OnDestroy()
    {
        if (_privilegesBeingRequested)
        {
            _privilegesBeingRequested = false;

            MLPrivilegesStarterKit.Stop();
        }
        MLInput.Stop();
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

    private void OnButtonDown(byte controllerId, MLInput.Controller.Button button)
    {
        if (button == MLInput.Controller.Button.Bumper)
        {

        }
    }

    private void OnCaptureRawImageComplete(byte[] imageData)
    {
        lock (_cameraLockObject)
        {
            _isCapturing = false;
        }
        if (stamp2Copy.ContainsKey(time_stamp))
        {
            return;
        }
        if (!makingSuggestion)
        {
            stamp2Copy[time_stamp] = Instantiate(copy_prefab, ctransform.position, ctransform.rotation);
            time_stamp++;
        }
        UploadFile(imageData, time_stamp - 1);
    }

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


}

