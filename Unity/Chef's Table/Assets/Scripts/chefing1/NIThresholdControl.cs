using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NIThresholdControl : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject mainCam;
    public float radius;
    public bool autoAdjustToViewPort = true;
    public float viewPortTimer = 5f;
    public float viewPortAngel = 120;
    public bool Lock = false;
    
    void Start()
    {
        Invoke("init", 1);
    }

    void init()
    {

        transform.position = new Vector3(transform.position.x, mainCam.transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (Lock) { return; }
        Vector3 camPos = mainCam.transform.position;
        Vector3 cam2Target = new Vector3(transform.position.x - camPos.x, 0, transform.position.z - camPos.z);
        if (cam2Target.magnitude > radius)
        {
            transform.position = camPos + radius / cam2Target.magnitude * cam2Target; // adjust only position
        }
        if (autoAdjustToViewPort)
        {
            if (!inViewPortNow())
            {
                viewPortTimer -= Time.deltaTime;
                if (viewPortTimer < 0)
                {
                    viewPortAutoAdjust();
                    viewPortTimer = 5f;
                }
            }
            else
            {
                viewPortTimer = 5f;
            }
        }
        transform.LookAt(mainCam.transform);
    }

    private bool inViewPortNow()
    {
        Vector3 cam2Interface = this.transform.position - mainCam.transform.position;
        cam2Interface.y = 0;
        Vector3 lookTo = mainCam.transform.forward;
        lookTo.y = 0;
        return Mathf.Acos(Vector3.Dot(lookTo.normalized, cam2Interface.normalized)) < Mathf.Deg2Rad * viewPortAngel / 2f;
    }

    public void viewPortAutoAdjust()
    {
        Vector3 cam2Interface = this.transform.position - mainCam.transform.position;
        cam2Interface.y = 0;
        Vector3 lookTo = mainCam.transform.forward;
        lookTo.y = 0;
        float height = mainCam.transform.position.y;
        float angleAjust = 0;
        angleAjust = Vector3.Cross(lookTo, cam2Interface).y > 0 ? viewPortAngel / 2 - 10 : -viewPortAngel / 2 + 10;
        Vector3 rotatedVector = Quaternion.AngleAxis(angleAjust, Vector3.up) * lookTo;
        this.transform.position = rotatedVector.normalized * radius + mainCam.transform.position;
    }

    public void changeLock()
    {
        Lock = !Lock;
    }

    public bool getLock()
    {
        return Lock;
    }
}
