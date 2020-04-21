using System.Collections;
using System.Collections.Generic;
using System.Timers;
using System;
using UnityEngine;

public class MainScheduler : MonoBehaviour
{
    // Start is called before the first frame update

    private List<List<Step>> tutorial = new List<List<Step>>();
    private int stepIndex = 0;

    // initialize the scheduler with hardcoded tutorial
    void hardcodeInit()
    {
        HashSet<string> utensils = new HashSet<string>();
        utensils.Add("Pot1");
        utensils.Add("Microwave");
        HashSet<string> ingredients = new HashSet<string>();
        ingredients.Add("water");
        ingredients.Add("bread");
        Step s = new Step("step1", 120, true, utensils, ingredients);
        List<Step> list = new List<Step>();
        list.Add(s);
        tutorial.Add(list);
    }

    void Start()
    {
        hardcodeInit();
    }

    // Update is called once per frame
    void Update()
    {
        List<Step> curr = tutorial[stepIndex];
        curr.ForEach(timerUpdate);
        
    }


   void timerUpdate(Step s)
    {
        float t = s.getTime();
        t -= Time.deltaTime;
        s.setTimer(t);
    }

    string GetTimeSpanWithSec(float seconds)
    {
        TimeSpan interval = TimeSpan.FromSeconds(seconds);
        return interval.ToString();
    }


    public string info()
    {
        string res = "";
        List<Step> curr = tutorial[stepIndex];
        foreach (Step s in curr)
        {
            res += s.getIdentifier() + " " + GetTimeSpanWithSec(s.getTime()) + " ";
        }
        return res;
    }

}
