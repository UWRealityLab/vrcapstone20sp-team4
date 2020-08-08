using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;
using UnityEngine;
using System.Threading;

public class MainScheduler2 : MonoBehaviour
{
    // Start is called before the first frame update


    // for bookkeeping and manipulation

    private InstructionList tutorial; // tutorial will be read only
    private int stepIndex = 0;
    private List<float> timerRecord = new List<float>();
    private bool timerPause = true;

    // for statistic
    private string chosenRecipe = "";
    private float totalTime = 0;


    // global states
    private bool tutorialStarts = false; // indicate if a user has choosen a tutorial
    private bool tutorialFinish = false;



    // simply reset everything.
    public void reset()
    {
        tutorial = null;
        stepIndex = 0;
        timerRecord.Clear();
        chosenRecipe = "";
        totalTime = 0;
        tutorialStarts = false;
        tutorialFinish = false;
    }

    public void addToTimer(float delta)
    {
        if (tutorialStarts)
        {
            timerRecord[stepIndex] += delta;
        }
    }

    public void subtractFromTimer(float delta)
    {
        if (tutorialStarts)
        {
            timerRecord[stepIndex] -= delta;
        }
    }

    // change timer status at the current step index for all substeps
    // 0 for pause, 1 for start, 2 reset timer and pause
    public void changeTimerStatus(bool pause)
    {
        timerPause = pause;
    }

    public bool isTutorialDone()
    {
        return tutorialFinish;
    }

    public List<string> getSummary()
    {
        List<String> res = new List<string>();
        res.Add(chosenRecipe);
        res.Add(GetTimeSpanWithSec(totalTime));
        return res;
    }

    // return a map of all info of the current step, null if no tutorial is selected
    public Dictionary<string, List<string>> getCurrentStepInfo()
    {
        if (!tutorialStarts) return null;
        Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
        // TODO: add utensils and ingredients
        dic.Add("description", new List<string>() { tutorial.steps[stepIndex].step});
        dic.Add("timer", new List<string>() { GetTimeSpanWithSec(tutorial.steps[stepIndex].timer) });
        dic.Add("recipe", new List<string>() { chosenRecipe });
        dic.Add("StepNum", new List<string>() { (stepIndex + 1) + "" });
        return dic;
    }

    // proceed to the next task in the list
    public void toNextStep()
    {
        stepIndex++;
        if (stepIndex >= timerRecord.Count)
        {
            tutorialFinish = true;
        }
    }

    public void toPreviousStep()
    {
        resetTimerRecord();
        if (stepIndex == 0) return;
        stepIndex--;
        tutorialFinish = false;
    }

    private void resetTimerRecord()
    {
        if (tutorial != null)
        {
            timerRecord.Clear();
            foreach(Instruction instruction in tutorial.steps)
            {
                timerRecord.Add(instruction.timer);
            }
            timerPause = true;
        }
    }

    // for user interface to call when a user select a recipe
    // name: name of the recipe
    public void startTutorial(StepsList wrapper)
    {
        try
        {
            tutorial = wrapper.result[0];
            tutorialStarts = true;
            resetTimerRecord();
        }
        catch (NullReferenceException e)
        {
            Debug.LogWarning(e);
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (tutorialStarts)
        {
            if (!timerPause)
            {
                timerRecord[stepIndex] = Math.Max(timerRecord[stepIndex] - Time.deltaTime, 0);
                
            }
        }

    }


    // for visualizing timers
    private string GetTimeSpanWithSec(float seconds)
    {
        TimeSpan interval = TimeSpan.FromSeconds(Math.Floor(seconds));
        return interval.ToString();
    }

}