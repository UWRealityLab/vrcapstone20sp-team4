using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerButtons : MonoBehaviour
{
    StopWatch stopWatch;

    private void Start()
    {
        GameObject sw = GameObject.Find("StopWatch");
        stopWatch = sw.GetComponent<StopWatch>();
    }
    public void clicked()
    {

    }
}
