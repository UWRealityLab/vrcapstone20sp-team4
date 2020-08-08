using UnityEngine;
using System.Net;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class GetInstructions : MonoBehaviour
{
    private const string API_KEY = "c17e83382cd44c48aba862f8f2999f73";
    private const float API_CHECK_MAXTIME = 10 * 60.0f;  // 10 minutes
    private const int RECIPE_NUMBER = 6;
    /*
    private string[] COOKING_METHODS = { "bake", "fry", "roast", "grill", "steam", "poach",
        "simmer", "broil", "blanch", "braise", "stew" };
    private string[] OPERATIONS = { "stir", "add", "combine" };
    */
    public GameObject RecipeSystem;
    public string Ingredients;
    private float apiCheckCountdown = API_CHECK_MAXTIME;
    Texture2D OperationImage;

    // Start is called before the first frame update
    void Start()
    {
        HandleInstructions();
    }

    // Update is called once per frame
    void Update()
    {
        apiCheckCountdown -= Time.deltaTime;
        if (apiCheckCountdown <= 0) {
            HandleInstructions();
            apiCheckCountdown = API_CHECK_MAXTIME;
        }
    }

    public async void HandleInstructions()
    {
        // Only for test purpose:
        List<PreviewRecipe> recipeList = (await GetRecipes()).result;
        List<int> recipeIds = new List<int>();
        for (int i = 0; i < RECIPE_NUMBER; i++) {
            recipeIds.Add(recipeList[i].id);
        }
        List<string> recipeTitles = new List<string>();
        for (int i = 0; i < RECIPE_NUMBER; i++) {
            recipeTitles.Add(recipeList[i].title);
        }
        List<string> recipeImages = new List<string>();
        for (int i = 0; i < RECIPE_NUMBER; i++) {
            recipeImages.Add(recipeList[i].image);
        }

        // To get a ingredient for preview:
        PreviewRecipe preview = recipeList[5];
        List<MissedIngredients> list = preview.missedIngredients;
        string ingredient = list[2].original;

        // To get instructions
        int recipeId = recipeIds[3];
        List<Instruction> steps = (await GetSteps(recipeId)).result[0].steps;
        Instruction s = steps[steps.Count - 1];

        TimePeriod timePeriod = s.length;
        int time = timePeriod.number;
        string unit = timePeriod.unit;
        if (time != 0 && unit != "") {
            // there is a time period for this step, e.g. time(5) + unit(minutes)
        } else {
            // there is no time period for this step. The number and unit are set to default values 0 and ""
        }
    }


    private async Task<PreviewRecipeList> GetRecipes()
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format(
            "https://api.spoonacular.com/recipes/findByIngredients?ingredients={0}&number={1}&apiKey={2}",
            Ingredients, RECIPE_NUMBER, API_KEY));
        HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync());
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        jsonResponse = "{\"result\":" + jsonResponse + "}";
        PreviewRecipeList info = JsonUtility.FromJson<PreviewRecipeList>(jsonResponse);
        return info;
    }

    private async Task<StepsList> GetSteps(int RecipeId)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
            String.Format("https://api.spoonacular.com/recipes/{0}/analyzedInstructions?apiKey={1}",
            RecipeId, API_KEY));
        HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync());
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        jsonResponse = "{\"result\":" + jsonResponse + "}";
        StepsList info = JsonUtility.FromJson<StepsList>(jsonResponse);
        return info;
    }

    /*
    private void GetOperationImage(string Step)
    {
        Boolean hasImage = false;
        for (int i = 0; i < COOKING_METHODS.Length; i++) {
            string operation = COOKING_METHODS[i];
            bool contains = Step.IndexOf(operation, StringComparison.OrdinalIgnoreCase) >= 0;
            if (contains) {
                hasImage = true;
                DisplayImage(operation);
                break;  // one image for each step
            }
        }
        if (!hasImage) {
            for (int i = 0; i < OPERATIONS.Length; i++) {
                string operation = OPERATIONS[i];
                bool contains = Step.IndexOf(operation, StringComparison.OrdinalIgnoreCase) >= 0;
                if (contains) {
                    DisplayImage(operation);
                    break;  // one image for each step
                }
            }
        }
    }

    private void DisplayImage(string operation)
    {
        string filename = "Sprites/" + operation;
        OperationImage = Resources.Load(filename) as Texture2D;
        GameObject rawImage = GameObject.Find("RawImage");
        rawImage.GetComponent<RawImage>().texture = OperationImage;
    }
    */
}
