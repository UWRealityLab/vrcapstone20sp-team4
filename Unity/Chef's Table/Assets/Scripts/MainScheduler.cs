using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MainScheduler : MonoBehaviour
{
    // Start is called before the first frame update

    GameObject applicationState;
    ApplicationState applicationScript;
    private List<List<Step>> tutorial = new List<List<Step>>();
    private List<List<float>> memory = new List<List<float>>();
    private Dictionary<string, int> indexTable = new Dictionary<string, int>();
    private int stepIndex = 0;
    private List<bool> timerStatus = new List<bool>(); // 0 for pause, 1 for start, 0 by default
    private List<GameObject> animations = new List<GameObject>();
    private GameObject currAnimation = null;
    private GameObject canvas;
    
    // change timer status at the current step index for all substeps
    // 0 for pause, 1 for start, 2 reset timer and pause
    public void changeTimerStatus(int status)
    {
        if (status != 0 && status != 1 && status != 2) return;
        if (status == 2)
        {
            List<Step> steps = tutorial[stepIndex];
            List<float> mems = memory[stepIndex];
            for (int i = 0; i < steps.Count; i++)
            {
                float time = mems[i];
                steps[i].setTimer(time);
            }
        }
        timerStatus[stepIndex] = status == 1 ? true : false;
    }

    // return a map of all info of the current step
    public Dictionary<string, List<string>> getCurrentStepInfo()
    {
        Step s = tutorial[stepIndex][0]; // sequential for now, get the only substep in step
        Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
        dic.Add("name", new List<string>() {s.getName()});
        dic.Add("utensils", s.getUtensilsSet());
        dic.Add("ingredients", s.getIngredientsSet());
        dic.Add("description", new List<string>() {s.getDescription()});
        dic.Add("timer", new List<string>() { GetTimeSpanWithSec(s.getTime()) });
        return dic;
    }

    // proceed to the next task in the list
    public void toNextStep()
    {
        List<Step> curr = tutorial[stepIndex];
        foreach (Step s in curr)
        {
            s.setTimer(0);
            s.setActionRequired(false);
        }
    }

    // return names of all tasks in the tutorial
    public List<string> getAllSteps()
    {
        List<string> nameList = new List<string>();
        foreach (List<Step> l in tutorial)
        {
            foreach(Step s in l)
            {
                nameList.Add(s.getName());
            }
        }
        return nameList;
    }

    // replay from a certain step
    // 1. can select only a certain step
    // 2. can replay all steps between previous and target
    // target: name of the step
    // replayInterval: indicator for option2
    public bool replay(string target, bool replayInterval)
    {
        // lock all timers
        for (int i = 0; i < timerStatus.Count; i++)
        {
            timerStatus[i] = false;
        }
        int index = indexTable[target];
        int bigIndex = index / 10 - 1;
        int smallIndex = index % 10 - 1;
        if (bigIndex > stepIndex) return false;
        // start going back
        if (!replayInterval)
        {
            List<Step> steps = tutorial[bigIndex];
            List<float> mems = memory[bigIndex];
            float time = mems[smallIndex];
            steps[smallIndex].setTimer(time);
            steps[smallIndex].setActionRequired(true);
        } else
        {
            for (int i = stepIndex; i >= bigIndex; i--)
            {
                List<Step> steps = tutorial[i];
                List<float> mems = memory[i];
                for (int j = 0; j < steps.Count; j++)
                {
                    float time = mems[j];
                    steps[j].setTimer(time);
                    steps[j].setActionRequired(true);
                }
            }
        }
        stepIndex = bigIndex;
        return true;
    }


    void xmlInit()
    {
        const string path = @"../Chef's Table/Assets/Resources/Tutorials/tutorial1.xml";
        XmlDocument doc = new XmlDocument();
        doc.Load(path);
        XmlNodeList ingredientList = doc.DocumentElement.GetElementsByTagName("ingredient");
        for (int i = 0; i < ingredientList.Count; i++)
        {
            XmlAttributeCollection attributesCollection = ingredientList[i].Attributes;
            string name = attributesCollection.GetNamedItem("name").Value;
            double quantity = Double.Parse(attributesCollection.GetNamedItem("quantity").Value);
            string unit = attributesCollection.GetNamedItem("unit").Value;
            Ingredient ing = new Ingredient(name, quantity, unit);
            applicationScript.name2Ingredient.Add(name, ing);
        }

        XmlNodeList utensilsList = doc.DocumentElement.GetElementsByTagName("utensil");
        for (int i = 0; i < utensilsList.Count; i++)
        {
            XmlAttributeCollection attributesCollection = utensilsList[i].Attributes;
            string name = attributesCollection.GetNamedItem("name").Value;
            Utensil uten = new Utensil(name);
            applicationScript.name2Utensil.Add(name, uten);
        }

        XmlNodeList substepList = doc.DocumentElement.GetElementsByTagName("substep");
        for (int i = 0; i < substepList.Count; i++)
        {
            List<Step> substeps = new List<Step>();
            List<float> substepsMem = new List<float>();
            XmlAttributeCollection attributesCollection = substepList[i].Attributes;
            //Debug.Log(attributesCollection.GetNamedItem("name").Value);
            int thisSeqNum = Int32.Parse(attributesCollection.GetNamedItem("seqNum").Value);
            string name = attributesCollection.GetNamedItem("name").Value;
            float timer = float.Parse(attributesCollection.GetNamedItem("timer").Value);
            string description = attributesCollection.GetNamedItem("description").Value;
            List<string> utensils = new List<string>(attributesCollection.GetNamedItem("utensils").Value.Split(','));
            List<string> ingredients = new List<string>(attributesCollection.GetNamedItem("ingredients").Value.Split(','));
            Step substep = new Step(name, timer, true, utensils, ingredients, description);
            substeps.Add(substep);
            substepsMem.Add(timer);
            indexTable.Add(name, thisSeqNum);
            while (i + 1 < substepList.Count)
            {
                XmlAttributeCollection nextAttributesCollection = substepList[i + 1].Attributes;
                int nextSeqNum = Int32.Parse(nextAttributesCollection.GetNamedItem("seqNum").Value);
                if ((thisSeqNum / 10) != (nextSeqNum / 10))
                {
                    break;
                }
                //Debug.Log(nextAttributesCollection.GetNamedItem("name").Value + "para");
                string nextName = nextAttributesCollection.GetNamedItem("name").Value;
                float nextTimer = float.Parse(nextAttributesCollection.GetNamedItem("timer").Value);
                string nextDescription = nextAttributesCollection.GetNamedItem("description").Value;
                List<string> nextUtensils = new List<string>(nextAttributesCollection.GetNamedItem("utensils").Value.Split(','));
                List<string> nextIngredients = new List<string>(nextAttributesCollection.GetNamedItem("ingredients").Value.Split(','));
                Step nextSubstep = new Step(nextName, nextTimer, true, nextUtensils, nextIngredients, nextDescription);
                substeps.Add(nextSubstep);
                substepsMem.Add(nextTimer);
                indexTable.Add(nextName, nextSeqNum);
                i++;

            }
            tutorial.Add(substeps);
            memory.Add(substepsMem);
            timerStatus.Add(false);
        }
    }

    void Start()
    {
        applicationState = GameObject.Find("ApplicationState");
        applicationScript = applicationState.GetComponent<ApplicationState>();
        xmlInit();
        // for (int i = 1; i <= tutorial.Count; i++) {
        for (int i = 1; i <= 7; i++) { // experimenting for now, that's why the 7, there should be tutorial.Count number of animations in Resources folder
            GameObject obj = Resources.Load("Animations/Step" + i) as GameObject;
            animations.Add(obj);
        }
    }

    // Update is called once per frame
    void Update()
    {
       //Debug.Log("tutorial Count: " + tutorial.Count);
        List<Step> curr = tutorial[stepIndex];
        curr.ForEach(timerUpdate);
        if (checkCompletion(curr))
        {
            // stepIndex = stepIndex + 1 < tutorial.Count ? stepIndex + 1 : stepIndex;
            stepIndex = stepIndex + 1;
            if (stepIndex == tutorial.Count) {
                updateText("Meal is ready!");
            } else {
                if (currAnimation != null) {
                    Destroy(currAnimation);
                }
                curr = tutorial[stepIndex];
                currAnimation = Instantiate(animations[stepIndex], GameObject.Find("AnimationSpawnLocation").transform.position, Quaternion.identity);
                updateText(curr[0].getDescription()); // no sub steps for now
            }
        }
    }

    // updates the text currently displayed on canvas
    void updateText(string text) {
        Text txt = GameObject.Find("MainInstructions").GetComponent<Text>();
        txt.text = text;
    }

    // check for a slot in interval, if all steps are confirmed finished
    bool checkCompletion(List<Step> curr)
    {
        foreach (Step s in curr)
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
        if (timerStatus[stepIndex])
        {
            float t = s.getTime();
            if (t > 0)
            {
                t -= Time.deltaTime;
            }
            t = Math.Max(t, 0);
            s.setTimer(t);
        }

    }

    // for visualizing timers
    string GetTimeSpanWithSec(float seconds)
    {

        TimeSpan interval = TimeSpan.FromSeconds(Math.Floor(seconds));
        return interval.ToString();
    }
}
