using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floating : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 startPos;
    private float time;
    void Start()
    {
        startPos = transform.position;
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        time = time % 360;
        gameObject.transform.position = startPos + new Vector3(0, 0.02f * Mathf.Sin(time), 0);
    }

    public void resetByMode(Vector3 newStartPos)
    {
        
        startPos = newStartPos + transform.parent.transform.TransformVector(new Vector3(0, 0.5f, 0));
        time = 0;
    }
}
