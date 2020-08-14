using UnityEngine;
using System.Net;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

public class GetInstructions : MonoBehaviour
{
    private const string API_KEY = "c17e83382cd44c48aba862f8f2999f73";
    private const float API_CHECK_MAXTIME = 2.0f;
    private const int RECIPE_NUMBER = 6;
    // public GameObject RecipeSystem;
    private string ingredientList = "";
    private float ingredientsCheckCountdown = API_CHECK_MAXTIME;
    public List<PreviewRecipe> RecipeList = null;
    private Dictionary<string, Dictionary<string, List<string>>> allPreviews = new Dictionary<string, Dictionary<string, List<string>>>();
    List<Instruction> steps = new List<Instruction>();
    ScanningInterfaceButton button;
    Boolean previewed = false;
    int time = 0;

    // Start is called before the first frame update
    void Start()
    {
        // button = GameObject.Find("ScanningInterface").GetComponent<ScanningInterfaceButton>();
        // Debug.Log("start: " + Ingredients);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (!previewed) {
            time++;
            ingredientsCheckCountdown -= 0.1f;
            if (ingredientsCheckCountdown <= 0) {
                Ingredients = button.getIngredients();
                Debug.Log(Ingredients);
                if (Ingredients.Length > 0) {
                    previewed = true;
                    HandlePreviews();
                }
            }
        }
        */
    }

    public Boolean GetIngredientsList(string list)
    {
        if (list.Length > 0) {
            ingredientList = list;
            HandlePreviews();
            return true;
        }
        return false;
    }

    public async void HandlePreviews()
    {
        if (ingredientList.Length > 0) {
            RecipeList = (await GetRecipes()).result;
        }
    }

    // Get the preview info for all recipes
    private void GetPreviewList()
    {
        if (RecipeList != null) {
            for (int i = 0; i < RECIPE_NUMBER; i++) {
                PreviewRecipe recipe = RecipeList[i];
                Dictionary<string, List<string>> recipeDict = new Dictionary<string, List<string>>();

                // info(0) - (id(0), title(1), image(2), imageType(3), likes(4))
                List<string> info = new List<string>();
                info.Add(recipe.id.ToString());
                info.Add(recipe.title);
                info.Add(recipe.image);
                info.Add(recipe.imgeType);  // typo
                info.Add(recipe.likes.ToString());
                recipeDict.Add("info", info);

                // usedIngredients(1)
                List<string> usedIngredients = new List<string>();
                List<UsedIngredients> used = recipe.usedIngredients;
                for (int j = 0; j < used.Count; j++) {
                    usedIngredients.Add(used[j].original);
                }
                recipeDict.Add("used", usedIngredients);

                // missedIngredients(1)
                List<string> missedIngredients = new List<string>();
                List<MissedIngredients> missed = recipe.missedIngredients;
                for (int j = 0; j < missed.Count; j++) {
                    missedIngredients.Add(missed[j].original);
                }
                recipeDict.Add("missed", missedIngredients);

                allPreviews.Add(recipe.title, recipeDict);
            }
        }
    }

    public Dictionary<string, Dictionary<string, List<string>>> GetAllPreviews()
    {
        return allPreviews;
    }

    public async void GetRecipeSteps(int recipeId)
    {
        steps = (await GetSteps(recipeId)).result[0].steps;
    }

    public List<Instruction> RecipeSteps()
    {
        return steps;
    }

    private async Task<PreviewRecipeList> GetRecipes()
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format(
            "https://api.spoonacular.com/recipes/findByIngredients?ingredients={0}&number={1}&apiKey={2}",
            ingredientList, RECIPE_NUMBER, API_KEY));
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

}
