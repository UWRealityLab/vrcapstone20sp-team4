using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;
using UnityEngine;

public class MainScheduler : MonoBehaviour
{
    // Start is called before the first frame update

    GameObject applicationState;
    ApplicationState applicationScript;
    private List<List<Step>> tutorial = new List<List<Step>>();
    private List<List<float>> memory = new List<List<float>>();
    private Dictionary<string, int> indexTable = new Dictionary<string, int>();
    private int stepIndex = 0;
    
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
            bool confirmNeeded = attributesCollection.GetNamedItem("confirmNeeded").Value == "1";
            string description = attributesCollection.GetNamedItem("description").Value;
            List<string> utensils = new List<string>(attributesCollection.GetNamedItem("utensils").Value.Split(','));
            List<string> ingredients = new List<string>(attributesCollection.GetNamedItem("ingredients").Value.Split(','));
            Step substep = new Step(name, timer, confirmNeeded, utensils, ingredients);
            substeps.Add(substep);
            float mem = confirmNeeded ? timer : -timer;
            substepsMem.Add(mem);
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
                bool nextConfirmNeeded = nextAttributesCollection.GetNamedItem("confimrNeeded").Value == "1";
                string nextDescription = nextAttributesCollection.GetNamedItem("description").Value;
                List<string> nextUtensils = new List<string>(nextAttributesCollection.GetNamedItem("utensils").Value.Split(','));
                List<string> nextIngredients = new List<string>(nextAttributesCollection.GetNamedItem("ingredients").Value.Split(','));
                Step nextSubstep = new Step(nextName, nextTimer, nextConfirmNeeded, nextUtensils, nextIngredients);
                substeps.Add(nextSubstep);
                float nextMem = nextConfirmNeeded ? nextTimer : -nextTimer;
                substepsMem.Add(nextMem);
                indexTable.Add(nextName, nextSeqNum);
                i++;

            }
            tutorial.Add(substeps);
            memory.Add(substepsMem);
        }
    }


    // initialize the scheduler with hardcoded tutorial
    void hardcodeInit()
    {
        List<string> utensils = new List<string>();
        utensils.Add("chopping boar");
        utensils.Add("oven");
        List<string> ingredients = new List<string>();
        ingredients.Add("potato");
        ingredients.Add("bread");
        Step s = new Step("step1", 10, false, utensils, ingredients);
        List<Step> list = new List<Step>();
        list.Add(s);
        List<float> mem = new List<float>();
        mem.Add(-10); // magnitude of the timer and sign for boolean
        memory.Add(mem);
        tutorial.Add(list);
        indexTable.Add(s.getName(), 11); // do not use zero indexing


        List<string> utensils1 = new List<string>();
        utensils1.Add("Pot1");
        utensils1.Add("Microwave");
        List<string> ingredients1 = new List<string>();
        ingredients1.Add("tomato");
        ingredients1.Add("bread");
        Step s1 = new Step("step2", 10, true, utensils1, ingredients1);
        List<Step> list1 = new List<Step>();
        list1.Add(s1);
        List<float> mem1 = new List<float>();
        mem1.Add(10); // magnitude of the timer and sign for boolean
        memory.Add(mem1);
        tutorial.Add(list1);
        indexTable.Add(s1.getName(), 21);

        List<string> utensils2 = new List<string>();
        utensils2.Add("Pot2");
        utensils2.Add("oven");
        List<string> ingredients2 = new List<string>();
        ingredients2.Add("rice");
        ingredients2.Add("steak");
        Step s2 = new Step("step3", 10, false, utensils2, ingredients2);
        List<Step> list2 = new List<Step>();
        list2.Add(s2);
        List<float> mem2 = new List<float>();
        mem2.Add(-10); // magnitude of the timer and sign for boolean
        memory.Add(mem2);
        tutorial.Add(list2);
        indexTable.Add(s2.getName(), 31);
    }

    void Start()
    {
        applicationState = GameObject.Find("ApplicationState");
        applicationScript = applicationState.GetComponent<ApplicationState>();
        //hardcodeInit();
        xmlInit();
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
            res += s.getName() + " " + GetTimeSpanWithSec(s.getTime()) + " ";
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
            List<float> mems = memory[bigIndex];
            float temp = mems[smallIndex];
            float time = Math.Abs(temp);
            bool actionRequired = temp > 0 ? true : false;
            steps[smallIndex].setTimer(time);
            steps[smallIndex].setActionRequired(actionRequired);
        } else
        {
            for (int i = stepIndex; i >= bigIndex; i--)
            {
                List<Step> steps = tutorial[i];
                List<float> mems = memory[i];
                for (int j = 0; j < steps.Count; j++)
                {
                    float temp = mems[j];
                    float time = Math.Abs(temp);
                    bool actionRequired = temp >= 0 ? true : false;
                    steps[j].setTimer(time);
                    steps[j].setActionRequired(actionRequired);
                }
            }
        }
        stepIndex = bigIndex;
        return true;
    }

}
