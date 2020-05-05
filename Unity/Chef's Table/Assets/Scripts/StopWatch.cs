using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Only support minutes for now.
public class StopWatch : MonoBehaviour
{
    float timer;
    float seconds;
    float minutes;
    bool stopped = false;
    float prevTimer;

    TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        timer = 60;
        prevTimer = 60;
        calculateTime();
    }

    // Update is called once per frame
    void Update()
    {
        displayTime();
        if (!stopped)
        {
            updateTime();
            calculateTime();
        }

    }

    public void setTime(float seconds)
    {
        timer = seconds;
        prevTimer = seconds;
    }

    public void start()
    {
        stopped = false;
    }

    public void pause()
    {
        stopped = true;
    }

    private void calculateTime()
    {
        seconds = (int) timer % 60;
        minutes = (int) timer / 60;
    }

    private void updateTime()
    {
        timer -= Time.deltaTime;
    }

    private void displayTime()
    {
        text.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}
