using Boo.Lang;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;

public class Raycast : MonoBehaviour
{
    private bool debug = true;   
    private Vector3 cPosition;
    private int timeStamp = 0;
    private bool prevDone = true;
    private string currClass = "";
    private Queue<container> detectionQueue = new Queue<container>();
    ApplicationState As;
    public GameObject debugPrefab;

    public GameObject mainScheduler;
    private MainScheduler2 mainScheduler2;

    private void Start()
    {
        As = GameObject.Find("ApplicationState").GetComponent<ApplicationState>();
        mainScheduler2 = mainScheduler.GetComponent<MainScheduler2>();
        cPosition = Vector3.zero;
    }

    // reset a camera position
    public void setCpoition(Vector3 cPosition)
    {
        detectionQueue.Clear();
        this.cPosition = cPosition;
    }

    // make raycast request sequentially if any has been issued by the pipeline
    private void Update()
    {
        if (prevDone && detectionQueue.Count > 0)
        {
            prevDone = false;
            Debug.Log("Set to false");
            container con = detectionQueue.Dequeue();
            currClass = con.name;
            Vector3 rcPoint = con.point;
            Vector3 direction = rcPoint - cPosition;
            MLRaycast.QueryParams _raycastParams = new MLRaycast.QueryParams
            {
                Position = cPosition,
                Direction = direction.normalized,
                UpVector = new Vector3(1, 0, 0),
                Width = 1,
                Height = 1
            };
            MLRaycast.Raycast(_raycastParams, HandleOnReceiveRaycast);
        }
    }

    // a helper class contains target name and position
    public class container
    {
        public string name;
        public Vector3 point;
        
        public container(string name, Vector3 point)
        {
            this.name = name;
            this.point = point;
        }
    }

    // for pipeline to call to make request
    public void makeRayCast2(Dictionary<string, Vector3> detections, bool debugMode, Vector3 cPosition) 
    {
        if (!MLRaycast.IsStarted)
        {
            MLRaycast.Start();
        }
        if (!prevDone || detectionQueue.Count != 0) {
            prevDone = true;
            return;
        }
        this.cPosition = cPosition;
        foreach (var detection in detections)
        {
            detectionQueue.Enqueue(new container(detection.Key, detection.Value));
        }
    }


    private void OnDestroy()
    {
        MLRaycast.Stop();
    }

    // when a hit is observed, show visual effect, update the state
    private void updateObjects(Vector3 point, Vector3 normal)
    {
        As.setLocation(currClass, point);
        prevDone = true;
        // Debug.Log("Raycast completed for " + currClass);
        if (debug && mainScheduler2.stepIndex == 0)
        {
            
            GameObject debugObject = Instantiate(debugPrefab, point, Quaternion.identity);
            debugObject.transform.LookAt(cPosition);
            debugObject.transform.FindChild("Canvas").FindChild("Text").gameObject.GetComponent<Text>().text = currClass;
            Destroy(debugObject, 3f); 
            
        }
        
    }

    // Use a callback to know when to run the NormalMaker() coroutine.
    void HandleOnReceiveRaycast(MLRaycast.ResultState state, UnityEngine.Vector3 point, Vector3 normal, float confidence)
    {
        if (state == MLRaycast.ResultState.HitObserved)
        {
            //StartCoroutine(NormalMarker(point, normal));
            updateObjects(point, normal);
        }
    }

}
