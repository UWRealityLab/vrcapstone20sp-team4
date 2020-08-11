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
            Debug.Log("make Raycast for: " + con.name);
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
        Debug.Log("raycast request made");
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
    // Instantiate the prefab at the given point.
    // Rotate the prefab to match given normal.
    // Wait 2 seconds then destroy the prefab.
    //private IEnumerator NormalMarker(Vector3 point, Vector3 normal)
    //{
    //    prevDone = true;
    //    Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);
    //    GameObject go = Instantiate(prefab, point, Quaternion.LookRotation(-(cPosition - point), Vector3.up));
    //    LineRenderer lr = go.AddComponent<LineRenderer>();
    //    Text t = go.transform.Find("Canvas/Text").gameObject.GetComponent<Text>();
    //    t.text = currClass;
    //    lr.material = new Material(Shader.Find("Sprites/Default"));
    //    lr.widthMultiplier = 0.05f;
    //    lr.SetPosition(0, cPosition);
    //    lr.SetPosition(1, point);
    //    yield return new WaitForSeconds(7f);
    //    Destroy(go);

    //}

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
