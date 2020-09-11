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
    // Camera's transform
   // public GameObject prefab;    // Cube prefab
    private Vector3 cPosition;
    private int timeStamp = 0;
    private bool prevDone = false;
    private string currClass = "";
    private Queue<container> detectionQueue = new Queue<container>();
    ApplicationState As;
    //[Range(0, 1)] public float x_ = 0.5f;
    //[Range(0, 1)] public float y_ = 0.5f;

    private void Start()
    {
        As = GameObject.Find("ApplicationState").GetComponent<ApplicationState>();
    }

    public void setCpoition(Vector3 cPosition)
    {
        this.cPosition = cPosition;
    }

    private void Update()
    {
        if (prevDone)
        {
            prevDone = false;
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
    public void makeRayCast2(Dictionary<string, Vector3> detections, bool debugMode) 
    {
        if (!MLRaycast.IsStarted)
        {
            MLRaycast.Start();
        }
        foreach (var detection in detections)
        {
            detectionQueue.Enqueue(new container(detection.Key, detection.Value));
        }
        prevDone = true;
        

    }


    private void OnDestroy()
    {
        MLRaycast.Stop();
    }

    private void updateObjects(Vector3 point, Vector3 normal)
    {
        prevDone = true;
        As.setLocation(currClass, point);
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
