using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;
using UnityEngine;
using System.Threading;

public class MainScheduler : MonoBehaviour
{
    // Start is called before the first frame update


    // for bookkeeping and manipulation
    private List<List<Step>> tutorial = new List<List<Step>>();
    private List<List<float>> memory = new List<List<float>>();
    private Dictionary<string, int> indexTable = new Dictionary<string, int>();
    private int stepIndex = 0;
    private List<bool> timerStatus = new List<bool>(); // 0 for pause, 1 for start, 0 by default

    // for statistic
    private string chosenRecipe = "";
    private float totalTime = 0;

    // global states
    private bool tutorialStarts = false; // indicate if a user has choosen a tutorial
    private bool tutorialFinish = false;
    private bool updateAnimation = true; // indicate if a new animation need to be played, change whenvever user move to a new step
    private string SelectedRecipe;
    public GameObject animationPlaySpace;
    private GameObject Animation;

    // for UI to call
    // recipe name to info map
    // info map pathtoxml pathToImage, serving, list of ingredient, list of utensils
    private Dictionary<string, Dictionary<string, List<string>>> allTutorials = new Dictionary<string, Dictionary<string, List<string>>>();
    public float animationDistance = 0.0f;

    // simply reset everything.
    public void reset()
    {
        tutorial.Clear();
        memory.Clear();
        indexTable.Clear();
        timerStatus.Clear();
        stepIndex = 0;
        tutorialStarts = false;
        tutorialFinish = false;
        updateAnimation = true;
        chosenRecipe = "";
        totalTime = 0;
}

    public void addToTimer()
    {
        foreach (Step s in tutorial[stepIndex])
        {
            s.setTimer(s.getTime() + 60);
        }
    }

    public void subtractFromTimer()
    {
        foreach (Step s in tutorial[stepIndex])
        {
            float current = s.getTime();
            current = current > 60 ? current - 60 : 0;
            s.setTimer(current);
        }
    }

    // change timer status at the current step index for all substeps
    // 0 for pause, 1 for start, 2 reset timer and pause
    public void changeTimerStatus(int status)
    {
        if (!tutorialStarts) return;
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
        Step s = tutorial[stepIndex][0]; // sequential for now, get the only substep in step
        Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
        dic.Add("name", new List<string>() { s.getName() });
        dic.Add("utensils", s.getUtensilsSet());
        dic.Add("ingredients", s.getIngredientsSet());
        dic.Add("description", new List<string>() { s.getDescription() });
        dic.Add("timer", new List<string>() { GetTimeSpanWithSec(s.getTime()) });
        dic.Add("stepIndex", new List<string>() { stepIndex.ToString() });
        dic.Add("recipe", new List<string>() { SelectedRecipe });
        return dic;
    }

    // proceed to the next task in the list
    public void toNextStep()
    {
        if (!tutorialStarts) return;
        List<Step> curr = tutorial[stepIndex];
        foreach (Step s in curr)
        {
            s.setTimer(0);
            s.setActionRequired(false);
        }
        updateAnimation = true;
    }

    public void toPreviousStep()
    {
        List<string> allSteps = getAllSteps();
        int target = stepIndex == 0 ? 0 : stepIndex - 1;
        string name = allSteps[target];
        replay(name, false);
    }

    // return names of all tasks in the tutorial
    // null if no tutorial is selected
    public List<string> getAllSteps()
    {
        if (!tutorialStarts) return null;
        List<string> nameList = new List<string>();
        foreach (List<Step> l in tutorial)
        {
            foreach (Step s in l)
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
        if (!tutorialStarts) return false;
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
        }
        else
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
        updateAnimation = true;
        return true;
    }

    public void previewAllTutorial2()
    {
        int i = 1;
        string filename = makeXmlName(i);
        TextAsset textAsset = (TextAsset)Resources.Load(filename);
        Debug.Log( filename);
        while (textAsset != null)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(textAsset.text);
            string name = doc.FirstChild.Attributes.GetNamedItem("name").Value;
            string pathToImage = "Image/tutorial" + i + "_image";
            string servings = doc.GetElementsByTagName("ingredients")[0].Attributes.GetNamedItem("servings").Value;
            List<string> ingredients = new List<string>();
            List<string> utensils = new List<string>();
            XmlNodeList ingList = doc.GetElementsByTagName("ingredient");
            for (int j = 0; j < ingList.Count; j++)
            {
                XmlAttributeCollection attributesCollection = ingList[j].Attributes;
                string ingName = attributesCollection.GetNamedItem("name").Value;
                ingredients.Add(ingName);
            }
            XmlNodeList utenList = doc.GetElementsByTagName("utensil");
            for (int j = 0; j < ingList.Count; j++)
            {
                XmlAttributeCollection attributesCollection = utenList[j].Attributes;
                string utenName = attributesCollection.GetNamedItem("name").Value;
                utensils.Add(utenName);
            }
            Dictionary<string, List<string>> currentRecipe = new Dictionary<string, List<string>>();
            currentRecipe.Add("pathToXml", new List<string>() { filename });
            currentRecipe.Add("pathToImage", new List<string>() { pathToImage });
            currentRecipe.Add("servings", new List<string>() { servings });
            currentRecipe.Add("ingredients", ingredients);
            currentRecipe.Add("utensils", utensils);
            allTutorials.Add(name, currentRecipe);
            
            i++;
            filename = makeXmlName(i);
            textAsset = (TextAsset)Resources.Load(filename);
        }
    }

   /* // preview all the recipe, store in a map( name : path)
    public void previewAllTutorial()
    {
        const string directory = @"../Chef's Table/Assets/Resources/Tutorials/";
        string[] files = Directory.GetFiles(directory);
        XmlDocument doc = new XmlDocument();
        int i = 1;
        foreach (string file in files)
        {
            if (file.Substring(file.Length - 4).Equals(".xml"))
            {
                doc.Load(file);
                
                string name = doc.FirstChild.Attributes.GetNamedItem("name").Value;
                string pathToImage = "../Chef's Table/Assets/Resources/Image/tutorial" + i + "_image";
                string servings = doc.GetElementsByTagName("ingredients")[0].Attributes.GetNamedItem("servings").Value;
                List<string> ingredients = new List<string>();
                List<string> utensils = new List<string>();
                XmlNodeList ingList = doc.GetElementsByTagName("ingredient");
                for (int j = 0; j < ingList.Count; j++)
                {
                    XmlAttributeCollection attributesCollection = ingList[j].Attributes;
                    string ingName = attributesCollection.GetNamedItem("name").Value;
                    ingredients.Add(ingName);
                }
                XmlNodeList utenList = doc.GetElementsByTagName("utensil");
                for (int j = 0; j < ingList.Count; j++)
                {
                    XmlAttributeCollection attributesCollection = utenList[j].Attributes;
                    string utenName = attributesCollection.GetNamedItem("name").Value;
                    utensils.Add(utenName);
                }
                Dictionary<string, List<string>> currentRecipe = new Dictionary<string, List<string>>();
                currentRecipe.Add("pathToXml", new List<string>(){ file });
                currentRecipe.Add("pathToImage", new List<string>() { pathToImage });
                currentRecipe.Add("servings", new List<string>() { servings });
                currentRecipe.Add("ingredients", ingredients);
                currentRecipe.Add("utensils", utensils);
                allTutorials.Add(name, currentRecipe);
                i++;
            }

        }
    }

    */
    // return map(name : path), users only want name
    public Dictionary<string, Dictionary<string, List<string>>> getAllTutorialPreview()
    {
        return allTutorials;
    }

    // for user interface to call when a user select a recipe
    // name: name of the recipe
    public void startTutorial(string name)
    {
        if (!allTutorials.ContainsKey(name))
        {
            Debug.LogError("invalid path to recipe file");
            return;
        }
        SelectedRecipe = name;
        string path = allTutorials[name]["pathToXml"][0];
        if (!tutorialStarts)
        {
            loadSelectedWithXml(path);
        }
        
        tutorialStarts = true;
    }

    // load program with a selected recipe
    void loadSelectedWithXml(string path)
    {
        // const string path = @"../Chef's Table/Assets/Resources/Tutorials/tutorial1.xml";
        XmlDocument doc = new XmlDocument();
        TextAsset textAsset = (TextAsset)Resources.Load(path);
        doc.LoadXml(textAsset.text);
        // XmlNode tutorialName = doc.DocumentElement.GetElementsByTagName("tutorial")[0];
        // chosenRecipe = tutorialName.Attributes.GetNamedItem("name").Value;
        chosenRecipe = "Breakfast Burrito";
        XmlNodeList ingredientList = doc.DocumentElement.GetElementsByTagName("ingredient");
        for (int i = 0; i < ingredientList.Count; i++)
        {
            XmlAttributeCollection attributesCollection = ingredientList[i].Attributes;
            string name = attributesCollection.GetNamedItem("name").Value;
            double quantity = Double.Parse(attributesCollection.GetNamedItem("quantity").Value);
            string unit = attributesCollection.GetNamedItem("unit").Value;
            Ingredient ing = new Ingredient(name, quantity, unit);
            // applicationScript.name2Ingredient.Add(name, ing);
        }

        XmlNodeList utensilsList = doc.DocumentElement.GetElementsByTagName("utensil");
        for (int i = 0; i < utensilsList.Count; i++)
        {
            XmlAttributeCollection attributesCollection = utensilsList[i].Attributes;
            string name = attributesCollection.GetNamedItem("name").Value;
            Utensil uten = new Utensil(name);
            // applicationScript.name2Utensil.Add(name, uten);
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
            Debug.Log(name);
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

    void handleAnimationUpdates()
    {
        if (updateAnimation)
        {
            updateAnimation = false;
            int step_num = stepIndex + 1;
            string pathToPrefab = "Animations/" + SelectedRecipe + "/step" + step_num;
            Destroy(Animation);
            UnityEngine.Object res_load = Resources.Load(pathToPrefab);
            if (res_load == null)
            {
                res_load = Resources.Load("Animations/default");
            }
            Animation = (GameObject)Instantiate(res_load, animationPlaySpace.transform.position + new Vector3(0, animationDistance, 0), Quaternion.identity);

        }
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (tutorialStarts)
        {
            if (!tutorialFinish)
            {
                totalTime += Time.deltaTime;
            }
            
            List<Step> curr = tutorial[stepIndex];
            curr.ForEach(timerUpdate);
            if (checkCompletion(curr))
            {

                stepIndex = stepIndex + 1 < tutorial.Count ? stepIndex + 1 : stepIndex;

            }
            //handleAnimationUpdates();
            if (stepIndex == tutorial.Count - 1)
            {
                tutorialFinish = true;
            } else
            {
                tutorialFinish = false;
            }
        }
        

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
                // set alarm sound if timer crossed 0.
                if (t <= 0)
                {
                    AudioSource timerClip = GameObject.Find("Timer").GetComponent<AudioSource>();
                    timerClip.loop = true;
                    AudioSource.PlayClipAtPoint(timerClip.clip, GameObject.Find("ClockText").transform.position);
                }
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

    string makeXmlName(int i)
    {
        return "Tutorials/tutorial" + i;
    }
}