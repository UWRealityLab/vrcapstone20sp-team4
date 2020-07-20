﻿using System.Collections;
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

        if (Input.GetKeyDown(KeyCode.Alpha1)) schedulerScript.replay("step1-1", false);
        if (Input.GetKeyDown(KeyCode.Alpha2)) schedulerScript.replay("step1-1", true);
        if (Input.GetKeyDown(KeyCode.Alpha3)) schedulerScript.replay("step2-1", false);
        if (Input.GetKeyDown(KeyCode.Alpha4)) schedulerScript.replay("step2-1", true);
        if (Input.GetKeyDown(KeyCode.Alpha5)) schedulerScript.replay("step3-1", false);
        if (Input.GetKeyDown(KeyCode.Alpha6)) schedulerScript.replay("step3-1", true);

        if (Input.GetKeyDown(KeyCode.Alpha7)) schedulerScript.changeTimerStatus(0); // pause
        if (Input.GetKeyDown(KeyCode.Alpha8)) schedulerScript.changeTimerStatus(1); // start
        if (Input.GetKeyDown(KeyCode.Alpha9)) schedulerScript.changeTimerStatus(2); // reset
        //Debug.Log(schedulerScript.info());
        // simulate consent
        if (Input.GetKeyDown(KeyCode.C)) schedulerScript.toNextStep();
        if (Input.GetKeyDown(KeyCode.S)) schedulerScript.startTutorial("fried rice");  // hardcode, a user selects that he wants to cook burrito
        //Dictionary<string, List<string>> info_dic = schedulerScript.getCurrentStepInfo();
        //if (info_dic == null)
        //{
        //    tmp.text = "";
        //    Dictionary<string, List<string>> allTutorials = schedulerScript.getAllTutorial();
        //    List<string> recipe_names = new List<string>(allTutorials.Keys);
        //    foreach (string name in recipe_names)
        //    {
        //        tmp.text += name + " ";
        //    }
        //}
        //else
        //{
        //    tmp.text = info_dic["timer"][0];
        //}


    }



}