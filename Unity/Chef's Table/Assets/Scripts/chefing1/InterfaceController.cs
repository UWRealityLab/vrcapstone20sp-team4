using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MagicLeapTools;

public class InterfaceController : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject onboarding;
    private GameObject onboardingInterface;
    private GameObject recipePreview;
    private GameObject preview;
    public GameObject previewPrefab;
    GameObject scheduler;
    MainScheduler schedulerScript;
    Dictionary<string, Dictionary<string, List<string>>> allTutorials;

    public void loadPreview(string name)
    {
        if (allTutorials.ContainsKey(name))
        {
            //preview.SetActive(true);
            TextMeshPro utenText = recipePreview.transform.Find("UtensilsCanvas").Find("Names").GetComponent<TextMeshPro>();
            string temp = "";
            List<string> utenList = allTutorials[name]["utensils"];
            foreach (string utensil in utenList)
            {
                temp += utensil + ", ";
            }
            temp = temp.Substring(0, temp.Length - 2);
            utenText.text = temp;

            temp = "";
            TextMeshPro ingText = recipePreview.transform.Find("IngredientsCanvas").Find("Names").GetComponent<TextMeshPro>();            List<string> ingList = allTutorials[name]["ingredients"];
            foreach (string ingredient in ingList)
            {
                temp += ingredient + ", ";
            }
            temp = temp.Substring(0, temp.Length - 2);
            ingText.text = temp;

            TextMeshPro recipeName = recipePreview.transform.Find("Canvas").Find("RecipeName").GetComponent<TextMeshPro>();
            recipeName.text = name;
            
            Texture2D tex = (Texture2D)Resources.Load(allTutorials[name]["pathToImage"][0]);
            recipePreview.GetComponentInChildren<RawImage>().texture = tex;
            
            /*
            GameObject serving = preview.transform.Find("serving").gameObject;
            TextMeshProUGUI servingText = serving.GetComponent<TextMeshProUGUI>();
            servingText.text = "Serving: " + allTutorials[name]["servings"][0];
            */
        }
    }

    void Start()
    {
        scheduler = GameObject.Find("Scheduler");
        schedulerScript = scheduler.GetComponent<MainScheduler>();
        schedulerScript.previewAllTutorial2();
        preview = (GameObject)Instantiate(previewPrefab, this.transform.position + new Vector3(0.8f, 0, 0), this.transform.rotation);
        preview.SetActive(false);
        preview.transform.SetParent(this.transform, true);
        onboarding = GameObject.Find("Onboarding");
        onboardingInterface = onboarding.transform.Find("OnboardingInterface").gameObject;
        recipePreview = onboarding.transform.Find("OnboardingPreview").gameObject;
        Invoke("delayStart", 0.5f);
    }

    void delayStart()
    {
        allTutorials = schedulerScript.getAllTutorialPreview();
        List<string> recipe_names = new List<string>(allTutorials.Keys);
        Debug.Log(recipe_names.Count);
        for (int i = 0; i < recipe_names.Count; i++)
        {
            GameObject recipePlate = onboardingInterface.transform.Find("RecipePlate" + i).gameObject;
            TextMeshPro name = recipePlate.GetComponentInChildren<TextMeshPro>();
            name.text = recipe_names[i];
            Texture2D tex = (Texture2D) Resources.Load(allTutorials[recipe_names[i]]["pathToImage"][0]);
            Debug.Log(allTutorials[recipe_names[i]]["pathToImage"][0]);
            recipePlate.GetComponentInChildren<RawImage>().texture = tex;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            loadPreview("breakfast burrito");
        }
    }
}