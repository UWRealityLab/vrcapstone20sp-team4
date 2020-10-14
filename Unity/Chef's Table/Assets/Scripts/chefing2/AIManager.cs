using System.Collections;
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

    // Update is called once per frame
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

    private void completeAndDelete(List<AI> toRemove)
    {
        foreach (var A in toRemove)
        {
            AIs.Remove(A);
            StartCoroutine(completionEffect(A));
        }
    }

    IEnumerator completionEffect(AI A)
    {
        ParticleSystem ps = A.getGo().GetComponent<ParticleSystem>();
        ps.startSpeed = 20;
        ps.Emit(200);
        yield return new WaitForSeconds(0.5f);
        Destroy(A.getGo());
    }

    IEnumerator addAIDelay(AI ai) {
        yield return new WaitForSeconds(1f);
        AIs.Add(ai);
    }

    public void addNewAI(Vector3 targetPosition)
    {
        Debug.Log("New task assigned for AI");
        Vector3 initPosition = mainCam.transform.forward * 0.5f + mainCam.transform.position; // 1 meter in front of the camera
        GameObject instance = Instantiate(AIPrefab, initPosition, Quaternion.identity);  
        //StartCoroutine(addAIDelay(new AI(targetPosition, instance)));
        AIs.Add(new AI(targetPosition, instance));
    }

    private void transformAI(AI A)
    {
        GameObject go = A.getGo();
        Vector3 velocity = currentVelocity(A);
        go.transform.Translate(velocity * speed, Space.World);
    }

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

    private bool inViewPort(GameObject go, ref bool inHorizontalViewPort)
    {
        Vector3 cam2Go = mainCam.transform.InverseTransformPoint(go.transform.position);
        float horizontalAngle = Mathf.Abs(Vector3.Angle(new Vector3(cam2Go.x, 0, cam2Go.z), Vector3.forward));
        inHorizontalViewPort = horizontalAngle < horizontalFov / 2;
        float verticalAngle = Mathf.Abs(Vector3.Angle(new Vector3(0, cam2Go.y, cam2Go.z), Vector3.forward));
        return horizontalAngle < horizontalFov / 2 && verticalAngle < verticalFov / 2;
    }
    
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
