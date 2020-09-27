using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading;
using MagicLeap.Core.StarterKit;
using System.Net;
using System.Text;
using UnityEngine.XR.MagicLeap;
using UnityEngine.Events;
using System.Linq;


public class ScanningInterfaceButton : MonoBehaviour
{
    AudioSource buttonClip;
    InterfaceManager interfaceManager;
    DetectionPipeline pipeline;
    GameObject getRecipeButton;
    GameObject startScanningButton;
    GameObject pauseButton;
    GameObject playButton;
    GameObject exitButton;
    GameObject searchButton;

    private string ingredientListString = "";

    private GetInstructions recipeApi;

    private ScanningInterfaceController controller;

    void Awake()
    {
        pipeline = GameObject.Find("pipeline").GetComponent<DetectionPipeline>();

        getRecipeButton = GameObject.Find("GetRecipes");
        startScanningButton = GameObject.Find("StartScanning");
        pauseButton = GameObject.Find("Pause");
        playButton = GameObject.Find("Play");
        exitButton = GameObject.Find("Exit");
        searchButton = GameObject.Find("KeyboardSearch");

        interfaceManager = GameObject.Find("InterfaceManager").GetComponent<InterfaceManager>();
        buttonClip = GameObject.Find("Button_Click").GetComponent<AudioSource>();
        recipeApi = GameObject.Find("RecipeAPI").GetComponent<GetInstructions>();
        controller = GameObject.Find("ScanningContainer").GetComponent<ScanningInterfaceController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hand" && interfaceManager.clickOk())
        {
            gameObject.GetComponent<Button>().onClick.Invoke();
            interfaceManager.clickButton();
        }
    }

    public void clicked()
    {
        if (name == "TrashButtonScript")
        {
            string whichIngredient = this.transform.parent.parent.gameObject.name;
            int index = whichIngredient.Last() - '0' - 1;;
            controller.removeIngredient(index);
            AudioSource.PlayClipAtPoint(buttonClip.clip, this.transform.parent.position);
        }
        else if (name == "StartScanningButton")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, startScanningButton.transform.position);
            GameObject.Find("BeginScanScreen").SetActive(false);
            interfaceManager.setActiveScanningActive(true);
            /*
            controller.clearMemory();
            controller.handleResponseStatus();
            GameObject ing = GameObject.Find("Ingredients");
            ing.SetActive(true);
            for (int i = 0; i < ing.transform.childCount; i++)
            {
                if (ing.transform.GetChild(i).gameObject.name == "title" || ing.transform.GetChild(i).gameObject.name == "Status")
                {
                    ing.transform.GetChild(i).gameObject.SetActive(true);
                } else
                {
                    ing.transform.GetChild(i).gameObject.SetActive(false);
                }

            }

            pipeline.startPipeline(true);
            */
        }
        else if (name == "GetRecipesButton")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, getRecipeButton.transform.position);
            ingredientListString = string.Join(",", controller.array());
            recipeApi.GetIngredientsList(ingredientListString);
            interfaceManager.setActiveOnboardingInterface(true);
            interfaceManager.setActiveScanningInterface(false);
            pipeline.stopPipeline();
        } 
        else if (name == "ClearAllButtonScript") {
            AudioSource.PlayClipAtPoint(buttonClip.clip, this.transform.parent.position);
            controller.clearMemory();
        }
        else if (name == "PauseButtonScript")
        {
            Debug.Log("enter script");
            AudioSource.PlayClipAtPoint(buttonClip.clip, pauseButton.transform.position);
            playButton.SetActive(true);
            pauseButton.SetActive(false);
            controller.pause();
        }
        else if (name == "PlayButtonScript")
        {
            Debug.Log("enter script  play");
            AudioSource.PlayClipAtPoint(buttonClip.clip, playButton.transform.position);
            playButton.SetActive(false);
            pauseButton.SetActive(true);
            controller.play();
        }
        else if (name == "ExitScript")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, exitButton.transform.position);
            interfaceManager.setActiveWelcomeInterface(true);
            interfaceManager.setActiveScanningInterface(false);
            pipeline.stopPipeline();
        }
        else if (name == "KeyboardScriptSearch")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, searchButton.transform.position);
            // TODO: show keyboard and hide ingredients
        }
        else
        {
            Debug.Log("Unknown button");
        }
    }
}