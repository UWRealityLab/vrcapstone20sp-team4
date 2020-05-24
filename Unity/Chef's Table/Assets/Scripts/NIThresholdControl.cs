using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NIThresholdControl : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject mainCam;
    public float radius;
    public float fieldDegree;

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
        Vector3 camDir_xz = new Vector3(mainCam.transform.rotation.x, 0, mainCam.transform.rotation.z);
        Vector3 cam2Target = new Vector3(transform.position.x - camPos.x, 0, transform.position.z - camPos.z);
        if (cam2Target.magnitude > radius)
        {
            transform.position = camPos + radius / cam2Target.magnitude * cam2Target; // adjust only position
        }
        transform.LookAt(mainCam.transform);
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
