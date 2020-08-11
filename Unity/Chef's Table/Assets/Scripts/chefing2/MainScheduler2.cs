﻿using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading;

public class MainScheduler2 : MonoBehaviour
{
    // Start is called before the first frame update


    // for bookkeeping and manipulation

    private List<Instruction> tutorial; // tutorial will be read only
    private int stepIndex = 0;
    private List<float> timerRecord = new List<float>();
    private bool timerPause = true;

    // for statistic
    private string chosenRecipe = "";
    private float totalTime = 0;


    // global states
    private bool tutorialStarts = false; // indicate if a user has choosen a tutorial
    private bool tutorialFinish = false;

    private ApplicationState As;
    GameObject recipeAPI;
    GetInstructions getRecipe;

    private List<Texture> imagesCurrentStep = new List<Texture>();


    private void Start()
    {
        As = GameObject.Find("ApplicationState").GetComponent<ApplicationState>();
        recipeAPI = GameObject.Find("RecipeAPI");
        getRecipe = recipeAPI.GetComponent<GetInstructions>();
    }

    // store the preview info
    private Dictionary<string, Dictionary<string, List<string>>> allTutorials = new Dictionary<string, Dictionary<string, List<string>>>();


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
        if (tutorialStarts) {
            timerRecord[stepIndex] += delta;
        }
    }

    public void subtractFromTimer(float delta)
    {
        if (tutorialStarts) {
            timerRecord[stepIndex] -= delta;
        }
    }

    // change timer status at the current step index for all substeps
    // 0 for pause, 1 for start, 2 reset timer
    public void changeTimerStatus(int status)
    {
        if (status == 0) {
            timerPause = true;
        } else if (status == 1) {
            timerPause = false;
        } else {
            resetTimerRecord();
        }
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
        /*
        dic.Add("utensils", tutorial.steps[stepIndex].equipment);
        dic.Add("ingredients", s.getIngredientsSet());
        dic.Add("description", new List<string>() { tutorial.steps[stepIndex].step});
        */
        float seconds = tutorial[stepIndex].length.number;
        string unit = tutorial[stepIndex].length.unit;
        if (unit.StartsWith("minute", StringComparison.OrdinalIgnoreCase)) {
            seconds *= 60;
        } else if (unit.StartsWith("hour", StringComparison.OrdinalIgnoreCase)) {
            seconds += 3600;
        }
        dic.Add("timer", new List<string>() { GetTimeSpanWithSec(seconds) });
        dic.Add("recipe", new List<string>() { chosenRecipe });
        dic.Add("StepNum", new List<string>() { (stepIndex + 1) + "" });
        return dic;
    }

    // proceed to the next task in the list
    public void toNextStep()
    {
        stepIndex++;
        if (stepIndex >= timerRecord.Count) {
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
        if (tutorial != null) {
            timerRecord.Clear();
            foreach (Instruction instruction in tutorial) {
                timerRecord.Add(instruction.length.number);
            }
            timerPause = true;
        }
    }

    public void startTutorial(string name)
    {
        if (!allTutorials.ContainsKey(name)) {
            Debug.LogError("invalid recipe entry");
            return;
        }
        int recipeId = Int32.Parse(allTutorials[name]["info"][0]);
        getRecipe.GetRecipeSteps(recipeId);
        Debug.Log("get steps! " + recipeId);
        Invoke("delayStartTutorial", 1.5f);
    }

    // for user interface to call when a user select a recipe
    // name: name of the recipe
    public void delayStartTutorial()
    {
        try {
            tutorial = getRecipe.RecipeSteps();
            Debug.Log("tutorial length: " + tutorial.Count);
            GetImagesForEachStep();
            tutorialStarts = true;
            resetTimerRecord();
        }
        catch (NullReferenceException e) {
            Debug.LogWarning(e);
        }
    }

    public void PreviewAllTutorial()
    {
        
        allTutorials = getRecipe.GetAllPreviews();
    }

    public Dictionary<string, Dictionary<string, List<string>>> GetAllTutorialPreview()
    {
        return allTutorials;
    }

    // construct a list of images of ingredient for the current step(stepIndex)
    public void GetImagesForEachStep()
    {
        List<Ingredients> li = tutorial[1].ingredients;
        for (int i = 0; i < li.Count; i++) {
            // https://spoonacular.com/cdn/ingredients_100x100/ranch-dressing.jpg
            string pathToImage = "https://spoonacular.com/cdn/ingredients_100x100/" + li[i].image;
            Debug.Log(pathToImage);
            GetTexture(pathToImage);
        }
    }

    IEnumerator GetTexture(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } else {
            Texture t = ((DownloadHandlerTexture)www.downloadHandler).texture;
            imagesCurrentStep.Add(t);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (tutorialStarts) {
            if (!timerPause) {
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

    public Vector3 getTimerLocation()
    {
        return As.criticalEquipmentLocation(tutorial[stepIndex].equipment);
    }
}