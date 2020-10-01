using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class RecipeMemory : MonoBehaviour
{
    private GameObject onboarding;
    private GameObject onboardingInterface;
    private GameObject recipePreview;

    private string default_recipe_name = "Omelette in a Bowl";

    public List<Instruction> steps = null;
    private Dictionary<string, List<string>> recipeDict = null;

    // Start is called before the first frame update
    void Start()
    {
        steps = new List<Instruction>();
        recipeDict = new Dictionary<string, List<string>>();

        onboarding = GameObject.Find("Onboarding");
        onboardingInterface = onboarding.transform.Find("OnboardingInterface").gameObject;
        recipePreview = onboarding.transform.Find("OnboardingPreview").gameObject;

        loadPreview();
        displayPreview();
        loadInstructions();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void loadPreview()
    {
        // fetch preview json
        string path = "Memory/omelette_preview_json";
        TextAsset file = Resources.Load<TextAsset>(path);
        string content = file.text;
        PreviewRecipe recipe = JsonUtility.FromJson<PreviewRecipe>(content);

        // construct dictionary for preview
        if (recipe != null) {
            List<string> info = new List<string>();
            info.Add(recipe.id.ToString());
            info.Add(recipe.title);
            info.Add(recipe.image);
            recipeDict.Add("info", info);

            List<string> usedIngredients = new List<string>();
            List<UsedIngredients> used = recipe.usedIngredients;
            for (int j = 0; j < used.Count; j++) {
                usedIngredients.Add(used[j].original);
            }
            recipeDict.Add("used", usedIngredients);

            List<string> missedIngredients = new List<string>();
            List<MissedIngredients> missed = recipe.missedIngredients;
            for (int j = 0; j < missed.Count; j++) {
                missedIngredients.Add(missed[j].original);
            }
            recipeDict.Add("missed", missedIngredients);
        }
    }

    private void displayPreview()
    {
        // display in recipe plate
        GameObject recipePlate = onboardingInterface.transform.Find("RecipePlate0").gameObject;
        TextMeshPro name = recipePlate.GetComponentInChildren<TextMeshPro>();
        name.text = default_recipe_name;
        string pathToImage = recipeDict["info"][2];
        StartCoroutine(GetTexture(pathToImage, recipePlate));

        // display used ingredients list
        string temp = "";
        TextMeshPro usedText = recipePreview.transform.Find("UsedIngredientsCanvas").Find("Names").GetComponent<TextMeshPro>();
        List<string> usedList = recipeDict["used"];
        if (usedList.Count == 0) {
            usedText.text = "-\r\n";
        } else {
            foreach (string ingredient in usedList) {
                temp += ingredient + "\r\n";
            }
            usedText.text = temp;
        }

        // display missed ingredients list
        temp = "";
        TextMeshPro missedText = recipePreview.transform.Find("MissedIngredientsCanvas").Find("Names").GetComponent<TextMeshPro>();
        List<string> missedList = recipeDict["missed"];
        if (missedList.Count == 0) {
            missedText.text = "-\r\n";
        } else {
            foreach (string ingredient in missedList) {
                temp += ingredient + "\r\n";
            }
            missedText.text = temp;
        }

        // display recipe name in preview
        TextMeshPro recipeName = recipePreview.transform.Find("Canvas").Find("RecipeName").GetComponent<TextMeshPro>();
        recipeName.text = default_recipe_name;

        // display recipe image in preview
        StartCoroutine(GetTexture(pathToImage, recipePreview));
    }

    IEnumerator GetTexture(string url, GameObject obj)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } else {
            obj.GetComponentInChildren<RawImage>().texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }
    }

    private void loadInstructions()
    {
        string path = "Memory/omelette_instruction_json";
        TextAsset file = Resources.Load<TextAsset>(path);
        string content = file.text;
        InstructionList info = JsonUtility.FromJson<InstructionList>(content);
        steps = info.steps;
    }

    public List<Instruction> RecipeSteps()
    {
        return steps;
    }

}
