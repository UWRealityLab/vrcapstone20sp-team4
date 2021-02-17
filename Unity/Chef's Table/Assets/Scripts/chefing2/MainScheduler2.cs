using System.Collections;
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
    public int stepIndex = 0;
    private List<float> timerRecord = new List<float>();
    private bool timerPause = true;
    private int totalStepNum = 0;

    // for statistic
    private string chosenRecipe = "";
    private float totalTime = 0;

    private int debug_print = 10;


    // global states
    public bool tutorialStarts = false; // indicate if a user has choosen a tutorial
    public bool tutorialFinish = false;

    private bool alarmOk = true;

    private ApplicationState As;
    // for instructions from the API
    GameObject recipeAPI;
    GetInstructions getRecipe;
    // for instructions from the memory
    GameObject memory;
    RecipeMemory getMemory;
    DetectionPipeline pipeline;

    private GameObject NEXT;
    private GameObject BACK;

    public GameObject TimerAudio;


    private List<Texture> imagesCurrentStep = new List<Texture>();

    // store the preview info
    private Dictionary<string, Dictionary<string, List<string>>> allTutorials = new Dictionary<string, Dictionary<string, List<string>>>();

    private void Start()
    {
        As = GameObject.Find("ApplicationState").GetComponent<ApplicationState>();
        recipeAPI = GameObject.Find("RecipeAPI");
        getRecipe = recipeAPI.GetComponent<GetInstructions>();
        memory = GameObject.Find("RecipeMemory");
        getMemory = memory.GetComponent<RecipeMemory>();
        pipeline = GameObject.Find("pipeline").GetComponent<DetectionPipeline>();
        BACK = GameObject.Find("HeadLockCanvas/NearInterface/BACK");
        NEXT = GameObject.Find("HeadLockCanvas/NearInterface/NEXT");
    }



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
        if (!tutorialStarts || stepIndex >= tutorial.Count) return null;
        Instruction cur = tutorial[stepIndex];
        Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
        // add action
        dic.Add("action", new List<string>() { cur.action });

        // add description
        dic.Add("description", new List<string>() { cur.step });

        // add ingredients
        List<string> ingredients = new List<string>();
        foreach (Ingredients ingre in cur.ingredients) {
            ingredients.Add(ingre.name);
        }
        dic.Add("ingredients", ingredients);

        // add ingredients measurement
        List<string> measurement = new List<string>();
        foreach (Ingredients ingre in cur.ingredients) {
            measurement.Add(ingre.measurement);
        }
        dic.Add("measurement", measurement);

        // add equipment
        List<string> equipment = new List<string>();
        foreach (Equipment equip in cur.equipment) {
            equipment.Add(equip.name);
        }
        dic.Add("equipment", equipment);

        // add timer
        float seconds = timerRecord[stepIndex];
        // string unit = tutorial[stepIndex].length.unit;
        dic.Add("timer", new List<string>() { GetTimeSpanWithSec(seconds) });
        dic.Add("recipe", new List<string>() { chosenRecipe });
        dic.Add("StepNum", new List<string>() { (stepIndex + 1) + "" });
        dic.Add("lastStepNum", new List<string>() { tutorial.Count + ""});
        return dic;
    }

    // proceed to the next task in the list
    public void toNextStep()
    {
        // Debug.Log("to next received");
        if (stepIndex + 2 >= tutorial.Count) {
            tutorialFinish = true;
            Debug.Log("finish");
            NEXT.SetActive(false);
        } else {
            NEXT.SetActive(true);
            BACK.SetActive(true);
        }
        stepIndex++;
        timerPause = true;
    }

    public void toPreviousStep()
    {
        resetTimerRecord();
        if (stepIndex == 0) return;
        if (stepIndex - 1 == 0) {
            BACK.SetActive(false);
        } else {
            NEXT.SetActive(true);
            BACK.SetActive(true);
        }
        stepIndex--;
        tutorialFinish = false;
        timerPause = true;
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
        NEXT.SetActive(true);
        BACK.SetActive(false);
        chosenRecipe = name;
        if (!name.ToLower().Contains("omelette"))
        {
            if (!allTutorials.ContainsKey(name))
            {
                Debug.LogError("invalid recipe entry");
                return;
            }
            int recipeId = Int32.Parse(allTutorials[name]["info"][0]);
            getRecipe.GetRecipeSteps(recipeId);
            Invoke("delayStartTutorial", 3f);
        } else
        {
            Debug.Log("omelette");
            startOmeletteTutorial();
        }

    }

    public void startOmeletteTutorial()
    {
        pipeline.startPipeline(false);
        try {
            tutorial = getMemory.RecipeSteps();
            Debug.Log("Tutorial length is " + tutorial.Count);
            totalStepNum = tutorial.Count;
            tutorialStarts = true;
            resetTimerRecord();
        }
        catch (NullReferenceException e) {
            Debug.LogWarning(e);
        }
    }

    // for user interface to call when a user select a recipe
    // name: name of the recipe
    public void delayStartTutorial()
    {
        try {
            tutorial = getRecipe.RecipeSteps();
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

    public Texture getCurrentStepImage()
    {
        return imagesCurrentStep[stepIndex];
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
                if (timerRecord[stepIndex] == 0f && alarmOk) {
                    StartCoroutine(playAlarm());
                    
                }
            }
            if (!tutorialFinish)
            {
                totalTime += Time.deltaTime;
            }
        }
    }

    IEnumerator playAlarm()
    {
        alarmOk = false;
        AudioSource.PlayClipAtPoint(TimerAudio.GetComponent<AudioSource>().clip, GameObject.Find("HeadLockCanvas").transform.position);
        yield return new WaitForSeconds(2);
        alarmOk = true;
    }

    // for visualizing timers
    private string GetTimeSpanWithSec(float seconds)
    {
        if (seconds == 0.0) {
            return "";
        }
        TimeSpan interval = TimeSpan.FromSeconds(Math.Floor(seconds));
        return interval.ToString();
    }


}