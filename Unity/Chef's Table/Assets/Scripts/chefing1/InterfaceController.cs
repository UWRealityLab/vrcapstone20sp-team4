using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
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
    MainScheduler2 schedulerScript;
    Dictionary<string, Dictionary<string, List<string>>> allTutorials = new Dictionary<string, Dictionary<string, List<string>>>();
    List<string> recipe_names = new List<string>();

    public void loadPreview(string name)
    {
        if (allTutorials.ContainsKey(name)) {
            //preview.SetActive(true);
            TextMeshPro usedText = recipePreview.transform.Find("UsedIngredientsCanvas").Find("Names").GetComponent<TextMeshPro>();
            string temp = "";
            List<string> usedList = allTutorials[name]["used"];
            foreach (string ingredient in usedList) {
                temp += ingredient + "\r\n";
            }
            usedText.text = temp;

            temp = "";
            TextMeshPro missedText = recipePreview.transform.Find("MissedIngredientsCanvas").Find("Names").GetComponent<TextMeshPro>();
            List<string> missedList = allTutorials[name]["missed"];
            foreach (string ingredient in missedList) {
                temp += ingredient + "\r\n";
            }
            missedText.text = temp;

            TextMeshPro recipeName = recipePreview.transform.Find("Canvas").Find("RecipeName").GetComponent<TextMeshPro>();
            recipeName.text = name;

            string pathToImage = allTutorials[name]["info"][2];
            StartCoroutine(GetTexture(pathToImage, recipePreview));
        }
    }

    void Start()
    {
        scheduler = GameObject.Find("Scheduler");
        schedulerScript = scheduler.GetComponent<MainScheduler2>();
        // schedulerScript.PreviewAllTutorial();
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
        schedulerScript.PreviewAllTutorial();
        allTutorials = schedulerScript.GetAllTutorialPreview();
        recipe_names = new List<string>(allTutorials.Keys);
        for (int i = 0; i < recipe_names.Count; i++) {
            GameObject recipePlate = onboardingInterface.transform.Find("RecipePlate" + i).gameObject;
            TextMeshPro name = recipePlate.GetComponentInChildren<TextMeshPro>();
            name.text = recipe_names[i];
            string pathToImage = allTutorials[name.text]["info"][2];
            StartCoroutine(GetTexture(pathToImage, recipePlate));
        }
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)) {
            loadPreview("breakfast burrito");
        }
    }
}