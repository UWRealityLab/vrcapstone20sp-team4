using Boo.Lang;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;

public class Raycast : MonoBehaviour
{

    public Transform ctransform; // Camera's transform
    public GameObject prefab;    // Cube prefab
    public GameObject ImagePlane;
    //[Range(0, 1)] public float x_ = 0.5f;
    //[Range(0, 1)] public float y_ = 0.5f;

    private void Update()
    {
        
    }

    public void makeRayCast(Dictionary<string, Vector2> detections, bool debugMode)
    {
        if (!MLRaycast.IsStarted)
        {
            MLRaycast.Start();
        }
        foreach(var detection in detections)
        {
            name = detection.Key;
            float x_ = detection.Value.x;
            float y_ = detection.Value.y;
            float x = (x_ - 0.5f) * ImagePlane.transform.localScale.x;
            float y = (y_ - 0.5f) * ImagePlane.transform.localScale.y;
            Vector3 local_center = ImagePlane.transform.InverseTransformPoint(0, 0, 0);
            Vector3 local_point = local_center + new Vector3(x, y, 0);
            Vector3 world_point = ImagePlane.transform.TransformPoint(local_point);
            Vector3 direction = world_point - ctransform.position;
            if (debugMode)  Debug.DrawRay(ctransform.position, direction.normalized, Color.green);
            MLRaycast.QueryParams _raycastParams = new MLRaycast.QueryParams
            {
                Position = ctransform.position,
                Direction = direction.normalized,
                UpVector = ctransform.up,
                Width = 1,
                Height = 1
            };
            MLRaycast.Raycast(_raycastParams, HandleOnReceiveRaycast);
        }
    }
    private void OnDestroy()
    {
        MLRaycast.Stop();
    }
    // Instantiate the prefab at the given point.
    // Rotate the prefab to match given normal.
    // Wait 2 seconds then destroy the prefab.
    private IEnumerator NormalMarker(Vector3 point, Vector3 normal)
    {
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);
        GameObject go = Instantiate(prefab, point, rotation);
        yield return new WaitForSeconds(0.1f);
        Destroy(go);
    }

    // Use a callback to know when to run the NormalMaker() coroutine.
    void HandleOnReceiveRaycast(MLRaycast.ResultState state, UnityEngine.Vector3 point, Vector3 normal, float confidence)
    {
        if (state == MLRaycast.ResultState.HitObserved)
        {
            StartCoroutine(NormalMarker(point, normal));
        }
    }

    // When the prefab is destroyed, stop MLWorldRays API from running.
}
