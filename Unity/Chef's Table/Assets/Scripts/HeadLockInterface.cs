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
        if (Input.GetKeyDown(KeyCode.Alpha1)) schedulerScript.replay("step1", false);
        if (Input.GetKeyDown(KeyCode.Alpha2)) schedulerScript.replay("step1", true);
        if (Input.GetKeyDown(KeyCode.Alpha3)) schedulerScript.replay("step2", false);
        if (Input.GetKeyDown(KeyCode.Alpha4)) schedulerScript.replay("step2", true);
        if (Input.GetKeyDown(KeyCode.Alpha5)) schedulerScript.replay("step3", false);
        if (Input.GetKeyDown(KeyCode.Alpha6)) schedulerScript.replay("step3", true);
        //Debug.Log(schedulerScript.info());
        // simulate consent
        if (Input.GetKeyDown(KeyCode.C)) schedulerScript.consentProceed();

        tmp.text = "Last Command: " + currentKey + " " + schedulerScript.info();

    }



}
