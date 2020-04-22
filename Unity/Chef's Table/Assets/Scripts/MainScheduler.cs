using System.Collections;
using System.Collections.Generic;
using System.Timers;
using System;
using UnityEngine;

public class MainScheduler : MonoBehaviour
{
    // Start is called before the first frame update

    private List<List<Step>> tutorial = new List<List<Step>>();
    private List<List<int>> memory = new List<List<int>>();
    private Dictionary<string, int> indexTable = new Dictionary<string, int>();
    private int stepIndex = 0;
    
    // TODO:
    // 1. store another copy of steps in case need to restore


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
        List<int> mem = new List<int>();
        mem.Add(-10); // magnitude of the timer and sign for boolean
        memory.Add(mem);
        tutorial.Add(list);
        indexTable.Add(s.getIdentifier(), 11); // do not use zero indexing


        HashSet<string> utensils1 = new HashSet<string>();
        utensils1.Add("Pot1");
        utensils1.Add("Microwave");
        HashSet<string> ingredients1 = new HashSet<string>();
        ingredients1.Add("tomato");
        ingredients1.Add("bread");
        Step s1 = new Step("step2", 10, true, utensils1, ingredients1);
        List<Step> list1 = new List<Step>();
        list1.Add(s1);
        List<int> mem1 = new List<int>();
        mem.Add(10); // magnitude of the timer and sign for boolean
        memory.Add(mem1);
        tutorial.Add(list1);
        indexTable.Add(s1.getIdentifier(), 21);

        HashSet<string> utensils2 = new HashSet<string>();
        utensils2.Add("Pot2");
        utensils2.Add("oven");
        HashSet<string> ingredients2 = new HashSet<string>();
        ingredients2.Add("rice");
        ingredients2.Add("steak");
        Step s2 = new Step("step2", 10, false, utensils2, ingredients2);
        List<Step> list2 = new List<Step>();
        list2.Add(s2);
        List<int> mem2 = new List<int>();
        mem.Add(-10); // magnitude of the timer and sign for boolean
        memory.Add(mem2);
        tutorial.Add(list2);
        indexTable.Add(s2.getIdentifier(), 31);
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

    // for interface to call
    public void consentProceed()
    {
        List<Step> curr = tutorial[stepIndex];
        foreach (Step s in curr)
        {
            s.setActionRequired(false);
        }
    }

    // replay from a certain step
    // 1. can select only a certain step
    // 2. can replay all steps between previous and target
    // target: name of the step
    // replayInterval: indicator for option2
    public bool replay(string target, bool replayInterval)
    {
        int index = indexTable[target];
        int bigIndex = index / 10 - 1;
        int smallIndex = index % 10 - 1;
        if (bigIndex > stepIndex) return false;
        // start going back
        if (!replayInterval)
        {
            List<Step> steps = tutorial[bigIndex];
            List<int> mems = memory[bigIndex];
            int temp = mems[smallIndex];
            float time = Math.Abs(temp);
            bool actionRequired = temp > 0 ? true : false;
            steps[smallIndex].setTimer(time);
            steps[smallIndex].setActionRequired(actionRequired);
        } else
        {
            for (int i = stepIndex; i >= bigIndex; i--)
            {
                List<Step> steps = tutorial[i];
                List<int> mems = memory[i];
                for (int j = 0; j < steps.Count; j++)
                {
                    int temp = mems[j];
                    float time = Math.Abs(temp);
                    bool actionRequired = temp > 0 ? true : false;
                    steps[j].setTimer(time);
                    steps[j].setActionRequired(actionRequired);
                }
            }
        }
        stepIndex = bigIndex;
        return true;
    }

}
