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
        utensils.Add("chopping boar");
        utensils.Add("oven");
        HashSet<string> ingredients = new HashSet<string>();
        ingredients.Add("potato");
        ingredients.Add("bread");
        Step s = new Step("step1", 10, false, utensils, ingredients);
        List<Step> list = new List<Step>();
        list.Add(s);
        tutorial.Add(list);

        HashSet<string> utensils1 = new HashSet<string>();
        utensils.Add("Pot1");
        utensils.Add("Microwave");
        HashSet<string> ingredients1 = new HashSet<string>();
        ingredients.Add("tomato");
        ingredients.Add("bread");
        Step s1 = new Step("step2", 10, false, utensils, ingredients);
        List<Step> list1 = new List<Step>();
        list1.Add(s1);
        tutorial.Add(list1);
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
        if (checkCompletion(curr))
        {
            
            stepIndex = stepIndex + 1 < tutorial.Count ? stepIndex + 1 : stepIndex;
            
        }
        Debug.Log(checkCompletion(curr));
        Debug.Log(stepIndex);
    }

    // check for a slot in interval, if all steps are confirmed finished
    bool checkCompletion(List<Step> curr)
    {
        foreach(Step s in curr)
        {
            if (s.getTime() > 0 || s.actionRequired())
            {
                // if (s.getTime() == 0) // send confimation by calling a function in interface.
                return false;
            }
        }
        return true;
    }

   void timerUpdate(Step s)
    {
        float t = s.getTime();
        if (t > 0)
        {
            t -= Time.deltaTime;
        } else
        {
            t = 0;
        }
        
        s.setTimer(t);
    }

    string GetTimeSpanWithSec(float seconds)
    {

        TimeSpan interval = TimeSpan.FromSeconds(Math.Floor(seconds));
        return interval.ToString();
    }


    public string info()
    {
        string res = "Step" + (stepIndex + 1);
        List<Step> curr = tutorial[stepIndex];
        foreach (Step s in curr)
        {
            res += s.getIdentifier() + " " + GetTimeSpanWithSec(s.getTime()) + " ";
        }
        return res;
    }

}
