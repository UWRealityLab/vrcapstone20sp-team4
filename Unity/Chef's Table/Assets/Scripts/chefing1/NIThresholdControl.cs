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

    private Material lockMat;
    private Material unlockMat;

    private List<GameObject> lockIcons;

    private bool goToPositionFlag = false;
    private Vector3 target;
    
    void Start()
    {
        lockIcons = new List<GameObject>();

        lockIcons.Add(GameObject.Find("HeadLockCanvas/NearInterface/Lock/IconAndText/Icon"));
        lockIcons.Add(GameObject.Find("HeadLockCanvas/WelcomeInterface/Lock/IconAndText/Icon"));
        lockIcons.Add(GameObject.Find("HeadLockCanvas/Onboarding/OnboardingInterface/Lock/IconAndText/Icon"));
        lockMat = Resources.Load("Mat/ButtonLockMat", typeof(Material)) as Material;
        unlockMat = Resources.Load("Mat/ButtonUnlockMat", typeof(Material)) as Material;
        Invoke("init", 1);
    }

    void init()
    {

        transform.position = mainCam.transform.position + mainCam.transform.forward*0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        //updateLock();
        if (Lock) { return; }
        if (!goToPositionFlag) {
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
        } else {         
            transform.position = Vector3.MoveTowards(transform.position, target, 0.02f);
            if (Vector3.Distance(target, transform.position) < 0.1f) {
                goToPositionFlag = false;
            }
        }
        
        transform.LookAt(mainCam.transform);
    }

    public void updateLock()
    {
        Material mat = Lock ? lockMat : unlockMat;
        for (int i = 0; i < lockIcons.Count; i++)
        {
            if (lockIcons[i].activeSelf) {
                lockIcons[i].GetComponent<Renderer>().material = mat;
            }    
        }
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


    public void goToPosition(Vector3 position) {
        if (!goToPositionFlag) {
            this.goToPositionFlag = true;
            target = position;
        }
        
    }
}
