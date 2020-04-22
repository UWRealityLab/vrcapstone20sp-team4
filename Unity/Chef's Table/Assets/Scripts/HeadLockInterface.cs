using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeadLockInterface : MonoBehaviour
{
    // Start is called before the first frame update

    public TextMeshProUGUI tmp;
    private string currentKey = "";
    GameObject scheduler;
    MainScheduler schedulerScript;
    void Start()
    {
        scheduler = GameObject.Find("Scheduler");
        schedulerScript = scheduler.GetComponent<MainScheduler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) currentKey = "W";
        if (Input.GetKeyDown(KeyCode.A)) currentKey = "A";
        if (Input.GetKeyDown(KeyCode.S)) currentKey = "S";
        if (Input.GetKeyDown(KeyCode.D)) currentKey = "D";
        Debug.Log(schedulerScript.info());
        tmp.text = "Last Command: " + currentKey + " " + schedulerScript.info();

    }



}
