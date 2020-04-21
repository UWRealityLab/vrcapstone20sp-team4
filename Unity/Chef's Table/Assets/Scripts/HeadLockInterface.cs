using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine;

public class HeadLockInterface : MonoBehaviour
{
    // Start is called before the first frame update

    public TextMeshProUGUI tmp;
    private string currentKey = "";
    private float timer = 0.0f;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.W)) currentKey = "W";
        if (Input.GetKeyDown(KeyCode.A)) currentKey = "A";
        if (Input.GetKeyDown(KeyCode.S)) currentKey = "S";
        if (Input.GetKeyDown(KeyCode.D)) currentKey = "D";
        tmp.text = "Last Command: " + currentKey + "current time: " + GetTimeSpanWithSec(timer);

    }

    public string GetTimeSpanWithSec(float seconds)
    {
        TimeSpan interval = TimeSpan.FromSeconds(seconds);
        return interval.ToString();
    }

}
