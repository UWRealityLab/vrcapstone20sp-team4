using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class NOIControl : MonoBehaviour
{
    public InterfaceManager interfaceManager;

    private GameObject recipePrefab;
    private GameObject previewPrefab;
    private MainScheduler schedulerScript;
    private Dictionary<string, Dictionary<string, List<string>>> allTutorials;
    private LinkedList<string> recipeNames;
    private LinkedListNode<string> recipeNode;

    // Start is called before the first frame update
    void Start()
    {
        recipePrefab = this.transform.Find("NIRecipe").gameObject;
        previewPrefab = this.transform.Find("NIPreview").gameObject;
        previewPrefab.SetActive(false);

        schedulerScript = GameObject.Find("Scheduler").GetComponent<MainScheduler>();
        schedulerScript.previewAllTutorial2();

        Invoke("delayStart", 0.5f);
    }

    private void delayStart()
    {
        allTutorials = schedulerScript.getAllTutorialPreview();
        recipeNames = new LinkedList<string>(allTutorials.Keys);
        recipeNode = recipeNames.First;
        populateRecipe(recipeNode.Value);
    }

    public void enableRecipe()
    {
        recipePrefab.SetActive(true);
        previewPrefab.SetActive(false);
    }

    public void confirm()
    {
        if (previewPrefab.activeSelf)
        {
            // preview is active, should go to the recipe
            enableRecipe();
            startRecipe();   
        } else
        {
            // preview is not active, display preview
            recipePrefab.SetActive(false);
            previewPrefab.SetActive(true);
            populatePreview(recipePrefab.GetComponentInChildren<TextMeshProUGUI>().text);
        }
    }

    public void startRecipe()
    {
        string recipe_name = recipePrefab.GetComponentInChildren<TextMeshProUGUI>().text;
        GameObject sche = GameObject.Find("Scheduler");
        MainScheduler ms = sche.GetComponent<MainScheduler>();
        ms.startTutorial(recipe_name);
        interfaceManager.setActiveOnboardingInterface(false);
        interfaceManager.setActiveNearInterface(true);
    }

    public void nextRecipe()
    {
        // if at the end of the list, wrap around to the front
        if (recipeNode.Next == null)
        {
            recipeNode = recipeNames.First;
        } else
        {
            recipeNode = recipeNode.Next;
        }
        populateRecipe(recipeNode.Value);
        populatePreview(recipeNode.Value);
    }

    public void prevRecipe()
    {
        if (recipeNode.Previous == null)
        {
            recipeNode = recipeNames.Last;
        }
        else
        {
            recipeNode = recipeNode.Previous;
        }
        populateRecipe(recipeNode.Value);
        populatePreview(recipeNode.Value);
    }

    private void populateRecipe(string name)
    {
        Texture2D tex = (Texture2D)Resources.Load(allTutorials[name]["pathToImage"][0]);
        recipePrefab.GetComponent<RawImage>().texture = tex;
        recipePrefab.GetComponentInChildren<TextMeshProUGUI>().text = name;
    }

    private void populatePreview(string name)
    {
        Debug.Log("=== Recipe Name: " + name);
        // Populate utensils
        GameObject utensils = previewPrefab.transform.Find("utensils").gameObject;
        TextMeshProUGUI utenText = utensils.GetComponent<TextMeshProUGUI>();
        string temp = "";
        List<string> utenList = allTutorials[name]["utensils"];
        foreach (string utensil in utenList)
        {
            temp += utensil + ", ";

        }
        temp = temp.Substring(0, temp.Length - 2);
        utenText.text = "Utensils needed: \n" + temp;

        // Populate Ingredients
        temp = "";
        GameObject ingredients = previewPrefab.transform.Find("ingredients").gameObject;
        TextMeshProUGUI ingText = ingredients.GetComponent<TextMeshProUGUI>();
        List<string> ingList = allTutorials[name]["ingredients"];
        foreach (string ingredient in ingList)
        {
            temp += ingredient + ", ";
        }
        temp = temp.Substring(0, temp.Length - 2);
        ingText.text = "Ingredients required: \n" + temp;

        // Populate recipe name
        GameObject recipeName = previewPrefab.transform.Find("recipe name").gameObject;
        TextMeshProUGUI recipeText = recipeName.GetComponent<TextMeshProUGUI>();
        recipeText.text = name;

        // Populate serving size
        GameObject serving = previewPrefab.transform.Find("serving").gameObject;
        TextMeshProUGUI servingText = serving.GetComponent<TextMeshProUGUI>();
        servingText.text = "Serving: " + allTutorials[name]["servings"][0];
    }
    
}
