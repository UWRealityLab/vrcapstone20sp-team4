﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    // Start is called before the first frame update
    private bool enabled = true;
    public GameObject mainCam;
    private float horizontalFov = 40;
    private float verticalFov = 20;
    public GameObject AIPrefab;
    private List<AI> AIs;
    public float speed = 0.02f;
    public float offsetZ = 1f;

    
    void Start()
    {
        AIs = new List<AI>();
    }

    // update current AI positions very frame
    void Update()
    {
        
        if (enabled)
        {
            List<AI> toRemove = new List<AI>();
            foreach (AI A in AIs)
            {
        
                if (!A.getGo().activeSelf)
                {
                    A.getGo().SetActive(true);
                }
                if (Vector3.Distance(A.getTarget(), A.getGo().transform.position) < 0.1f)
                {
                   toRemove.Add(A);
                }
                else
                {
                   transformAI(A);
                }
                transformAI(A);
             
            }
            completeAndDelete(toRemove);
    
        } else
        {
            foreach (AI A in AIs)
            {
                A.getGo().SetActive(false);
            }
        }
    }

    // destroy an AI when it finishes a task
    private void completeAndDelete(List<AI> toRemove)
    {

        foreach (var A in toRemove)
        {
            if (AIs.Contains(A)) {
                AIs.Remove(A);
            }
            
            StartCoroutine(completionEffect(A));
        }
        
        toRemove.Clear();
    }

    // visual effect when an AI is deleted
    IEnumerator completionEffect(AI A)
    {
        ParticleSystem ps = A.getGo().GetComponent<ParticleSystem>();
        ps.startSpeed = 20;
        ps.Emit(200);
        yield return new WaitForSeconds(0.5f);
        Destroy(A.getGo());
    }

    // init a new AI with a task
    public void addNewAI(Vector3 targetPosition)
    {
        Vector3 initPosition = mainCam.transform.forward * 0.5f + mainCam.transform.position; // 1 meter in front of the camera
        GameObject instance = Instantiate(AIPrefab, initPosition, Quaternion.identity);  
        //StartCoroutine(addAIDelay(new AI(targetPosition, instance)));
        foreach (var AI in AIs) {
            ParticleSystem ps = AI.getGo().GetComponent<ParticleSystem>();
            Destroy(AI.getGo());
        }
        AIs.Clear();
        AIs.Add(new AI(targetPosition, instance));
    }

    // basic function for moving an AI
    private void transformAI(AI A)
    {
        GameObject go = A.getGo();
        Vector3 velocity = currentVelocity(A);
        go.transform.Translate(velocity * speed, Space.World);
    }

    // helper function calculates direction of the AI
    private Vector3 currentVelocity(AI A)
    {
        GameObject go = A.getGo();
        bool inHorizontalViewPort = true;
        if (inViewPort(go, ref inHorizontalViewPort)) 
        {
            return (A.getTarget() - go.transform.position).normalized; // if it is in the viewport, then simply move towards the target.
        } else
        {
            // bring everything to local space
            Vector3 cam2Target = mainCam.transform.InverseTransformPoint(A.getTarget()).normalized;
            float x, y, z;
            Vector3 left, right, up, down;
            left = Quaternion.AngleAxis(-horizontalFov / 2, Vector3.up) * Vector3.forward;
            right = Quaternion.AngleAxis(horizontalFov / 2, Vector3.up) * Vector3.forward;
            up = Quaternion.AngleAxis(-verticalFov / 2, Vector3.right) * Vector3.forward;
            down = Quaternion.AngleAxis(verticalFov / 2, Vector3.right) * Vector3.forward;
            z = offsetZ;
            if (inHorizontalViewPort) // in horizontal but not in verticle
            {
                x = cam2Target.x;
                // find up or down
                Vector3 yzProjection = new Vector3(0, cam2Target.y, cam2Target.z);
                Vector3 crossRes = Vector3.Cross(Vector3.forward, yzProjection);
                Vector3 upOrDown = crossRes.x > 0 ? up : down;
                y = upOrDown.y * offsetZ / upOrDown.z;
            } else // not in horizontal, then restrict the indicator to only left and right
            {
                y = cam2Target.y;
                // find left or right
                Vector3 xzProjection = new Vector3(cam2Target.x, 0, cam2Target.z);
                Vector3 crossRes = Vector3.Cross(Vector3.forward, xzProjection);
                Vector3 leftOrRight = crossRes.y > 0 ? left : right;
                x = leftOrRight.x * offsetZ / leftOrRight.z;
            }

            return (mainCam.transform.TransformPoint(new Vector3(x, y, z)) - go.transform.position).normalized;
            //return (mainCam.transform.TransformPoint((new Vector3(x, y, z)) - go.transform.position).normalized);
        }
    }

    // helper function to verify if the AI can be seen
    private bool inViewPort(GameObject go, ref bool inHorizontalViewPort)
    {
        Vector3 cam2Go = mainCam.transform.InverseTransformPoint(go.transform.position);
        float horizontalAngle = Mathf.Abs(Vector3.Angle(new Vector3(cam2Go.x, 0, cam2Go.z), Vector3.forward));
        inHorizontalViewPort = horizontalAngle < horizontalFov / 2;
        float verticalAngle = Mathf.Abs(Vector3.Angle(new Vector3(0, cam2Go.y, cam2Go.z), Vector3.forward));
        return horizontalAngle < horizontalFov / 2 && verticalAngle < verticalFov / 2;
    }
    
    // the AI class
    public class AI
    {
        private Vector3 position;
        private GameObject ai;

        public AI(Vector3 position, GameObject instance)
        {
            this.position = position;
            this.ai = instance;
        }
        
        public Vector3 getTarget()
        {
            return position;
        }

        public GameObject getGo()
        {
            return ai;
        }

        public void setTarget(Vector3 position)
        {
            this.position = position;
        }
    }
}
