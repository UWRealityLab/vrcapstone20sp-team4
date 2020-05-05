using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MagicLeapTools;

public class InterfaceController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject recipePrefab;
    public GameObject previewPrefab;
    private GameObject preview;
    GameObject scheduler;
    MainScheduler schedulerScript;
    Dictionary<string, Dictionary<string, List<string>>> allTutorials;

    public void loadPreview(string name)
    {
        if (allTutorials.ContainsKey(name))
        {
            preview.SetActive(true);
            GameObject utensils = preview.transform.Find("utensils").gameObject;
            // utensils.SetActive(false);
            TextMeshProUGUI utenText = utensils.GetComponent<TextMeshProUGUI>();
            //TextMeshProUGUI utenText = tmp[0];
            string temp = "";
            List<string> utenList = allTutorials[name]["utensils"];
            foreach (string utensil in utenList)
            {
                temp += utensil + ", ";

            }
            temp = temp.Substring(0, temp.Length - 2);
            utenText.text = "Utensils needed: \n" + temp;

            temp = "";
            GameObject ingredients = preview.transform.Find("ingredients").gameObject;
            TextMeshProUGUI ingText = ingredients.GetComponent<TextMeshProUGUI>();
            List<string> ingList = allTutorials[name]["ingredients"];
            foreach (string ingredient in ingList)
            {
                temp += ingredient + ", ";
            }
            temp = temp.Substring(0, temp.Length - 2);
            ingText.text = "Ingredients required: \n" + temp;

            GameObject recipeName = preview.transform.Find("recipe name").gameObject;
            TextMeshProUGUI recipeText = recipeName.GetComponent<TextMeshProUGUI>();
            recipeText.text = name;

            GameObject serving = preview.transform.Find("serving").gameObject;
            TextMeshProUGUI servingText = serving.GetComponent<TextMeshProUGUI>();
            servingText.text = "Serving: " + allTutorials[name]["servings"][0];
        }
        return;
    }

    void Start()
    {
        // loadAnimationTest();

        scheduler = GameObject.Find("Scheduler");
        schedulerScript = scheduler.GetComponent<MainScheduler>();
        schedulerScript.previewAllTutorial();
        preview = (GameObject)Instantiate(previewPrefab, this.transform.position + new Vector3(0.9f, 0, 0), this.transform.rotation);
        preview.SetActive(false);
        preview.transform.SetParent(this.transform, true);
        Invoke("delayStart", 0.5f);

    }

    void delayStart()
    {

        allTutorials = schedulerScript.getAllTutorialPreview();
        List<string> recipe_names = new List<string>(allTutorials.Keys);
        Debug.Log(recipe_names.Count);
        foreach (string s in recipe_names)
        {
            Debug.Log(s);
        }
        for (int i = 0; i < recipe_names.Count; i++)
        {
            Vector3 offset1 = new Vector3(0.15f, 0, 0);
            Vector3 offset2 = new Vector3(0, 0.34f, 0);
            Vector3 pos = i % 2 == 0 ? this.transform.position - offset1 : this.transform.position + offset1;
            pos = pos - i / 2 * offset2;
            GameObject go = (GameObject)Instantiate(recipePrefab, pos, this.transform.rotation);
            go.tag = "RecipeButton";
            go.transform.SetParent(this.transform, true);
            go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            TextMeshProUGUI[] tmp = go.GetComponentsInChildren<TextMeshProUGUI>();
            tmp[0].text = recipe_names[i];
            Button b = go.GetComponent<Button>();
            // string captured = recipe_names[i];
            PointerReceiver pr = go.GetComponent<PointerReceiver>();
            pr.recipe_name = recipe_names[i];
            Texture2D tex = loadImage(allTutorials[recipe_names[i]]["pathToImage"][0]);
            go.GetComponent<RawImage>().texture = tex;
        }
    }

    void loadAnimationTest()
    {
        GameObject animation = (GameObject)Instantiate(Resources.Load("Animations/tutorial1/step8"), new Vector3(0, 0, 0), Quaternion.identity);
    }

    Texture2D loadImage(string pathToImage)
    {
        Texture2D tex = null;
        byte[] fileData;
        string completePath;
        if (File.Exists(pathToImage + ".png"))
        {
            completePath = pathToImage + ".png";
        }
        else if (File.Exists(pathToImage + ".jpg"))
        {
            completePath = pathToImage + ".jpg";
        }
        else
        {
            Debug.LogError("Image file not found: " + pathToImage + " .jpg/.png");
            return tex;
        }
        fileData = File.ReadAllBytes(completePath);
        tex = new Texture2D(2, 2);
        tex.LoadImage(fileData);
        return tex;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            loadPreview("breakfast burrito");
        }

    }
}