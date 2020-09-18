﻿using Boo.Lang;
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
    private bool prevDone = false;
    private string currClass = "";
    private Queue<container> detectionQueue = new Queue<container>();
    ApplicationState As;
    public GameObject debugPrefab;
    //[Range(0, 1)] public float x_ = 0.5f;
    //[Range(0, 1)] public float y_ = 0.5f;

    private void Start()
    {
        As = GameObject.Find("ApplicationState").GetComponent<ApplicationState>();
    }

    public void setCpoition(Vector3 cPosition)
    {
        if (detectionQueue.Count > 0 && debug)
        {
            Debug.Log("constraints violated, leftover job: " + detectionQueue.Count);
        }
        detectionQueue.Clear();
        this.cPosition = cPosition;
    }

    private void Update()
    {
        // Debug.Log("update raycast");
        if (prevDone && detectionQueue.Count > 0)
        {
            Debug.Log("update");
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
            Debug.Log("handle on receive raycast");
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
        if (debug) Debug.Log("raycast request made: " + detectionQueue.Count);
        prevDone = true;
        

    }


    private void OnDestroy()
    {
        MLRaycast.Stop();
    }

    private void updateObjects(Vector3 point, Vector3 normal)
    {
        Debug.Log("update objects");
        As.setLocation(currClass, point);
        if (debug)
        {
            // As.setLocation(currClass, point);
            GameObject debugObject = Instantiate(debugPrefab, point, Quaternion.identity);
            debugObject.transform.LookAt(cPosition);
            debugObject.transform.FindChild("Canvas").FindChild("Text").gameObject.GetComponent<Text>().text = currClass;
            Destroy(debugObject, 3f);
        }
        prevDone = true;
    }

    // Use a callback to know when to run the NormalMaker() coroutine.
    void HandleOnReceiveRaycast(MLRaycast.ResultState state, UnityEngine.Vector3 point, Vector3 normal, float confidence)
    {
        Debug.Log("call handle");
        if (state == MLRaycast.ResultState.HitObserved)
        {
            //StartCoroutine(NormalMarker(point, normal));
            Debug.Log("call update objects");
            updateObjects(point, normal);
        }
    }

}
