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
    GameObject scheduler;
    MainScheduler schedulerScript;
    void Start()
    {
        Invoke("delayStart", 2.0f);
    }

    void delayStart()
    {
        scheduler = GameObject.Find("Scheduler");
        schedulerScript = scheduler.GetComponent<MainScheduler>();
        Dictionary<string, List<string>> allTutorials = schedulerScript.getAllTutorial();
        List<string> recipe_names = new List<string>(allTutorials.Keys);
        Debug.Log(recipe_names.Count);
        for (int i = 0; i < recipe_names.Count; i++)
        {
            Vector3 offset1 = new Vector3(0.35f, 0, 0);
            Vector3 offset2 = new Vector3(0, 0.8f, 0);
            Vector3 pos = i % 2 == 0 ? this.transform.position - offset1 : this.transform.position + offset1;
            pos = pos - i / 2 * offset2;
            GameObject go = (GameObject)Instantiate(recipePrefab, pos, this.transform.rotation);
            go.transform.SetParent(this.transform, true);
            go.transform.localScale = new Vector3(1, 1, 1);
            TextMeshProUGUI[] tmp = go.GetComponentsInChildren<TextMeshProUGUI>();
            tmp[0].text = recipe_names[i];
            Button b = go.GetComponent<Button>(); 
            // string captured = recipe_names[i];
            PointerReceiver pr = go.GetComponent<PointerReceiver>();
            pr.recipe_name = recipe_names[i];
            Texture2D tex = loadImage(allTutorials[recipe_names[i]][1]);
            go.GetComponent<RawImage>().texture = tex;
           // b.onClick.AddListener(() => handleOnBoardingClick(captured));
        }
    }

    //void handleOnBoardingClick(string recipeName)
    //{
    //    schedulerScript.startTutorial(recipeName);
    //} 

    // Update is called once per frame

    Texture2D loadImage(string pathToImage)
    {
        Texture2D tex = null;
        byte[] fileData;
        string completePath;
        if (File.Exists(pathToImage + ".png"))
        {
            completePath = pathToImage + ".png";
        } else if (File.Exists(pathToImage + ".jpg"))
        {
            completePath = pathToImage + ".jpg";
        } else
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
        
    }
}
