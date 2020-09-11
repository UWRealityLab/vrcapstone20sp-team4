﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.Video;

public class VisualCueManager : MonoBehaviour
{
    // Start is called before the first frame update
    private VisualCueTask currTask;
    private ApplicationState appState;
    private Dictionary<string, VideoClip> actionsCues = new Dictionary<string, VideoClip>();
    public GameObject equipmentIndicator;
    public GameObject ingredientIndicator;
    private float criticalEquipmentUpdateTimer = 0;
    public Dictionary<string, GameObject> nameToObject = new Dictionary<string, GameObject>();
    void Start()
    {
        appState = GameObject.Find("ApplicationState").GetComponent<ApplicationState>();
        actionsCues["cut"] = Resources.Load<VideoClip>("cutting");
        actionsCues["crack"] = Resources.Load<VideoClip>("egg_cracking");
        actionsCues["heat"] = Resources.Load<VideoClip>("heating");
        actionsCues["melt"] = Resources.Load<VideoClip>("melting");
        actionsCues["mix"] = Resources.Load<VideoClip>("mixing");
        actionsCues["slice"] = Resources.Load<VideoClip>("slicing");
        actionsCues["spread"] = Resources.Load<VideoClip>("spread");
        actionsCues["sprinkle"] = Resources.Load<VideoClip>("sprinkle");
    }



    // Update is called once per frame
    void Update()
    {
        if (currTask != null)
        {
            criticalEquipmentUpdateTimer = Mathf.Max(0, criticalEquipmentUpdateTimer - Time.deltaTime);
            if (criticalEquipmentUpdateTimer == 0)
            {

            }
        }
    }

    public void setTasks(VisualCueTask task)
    {
        // clean all state before setting a new task
        foreach (string name in nameToObject.Keys)
        {
            Destroy(nameToObject[name]);
        }
        nameToObject.Clear();
        criticalEquipmentUpdateTimer = 0;
        currTask = task;
    }

    

    public class VisualCueTask
    {
        public string action;
        public List<string> equipment;
        public List<string> ingredients;
    }



}