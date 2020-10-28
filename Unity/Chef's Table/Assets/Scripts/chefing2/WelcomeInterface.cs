using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WelcomeInterface : MonoBehaviour
{
    NIThresholdControl NIControl;
    AudioSource buttonClip;
    InterfaceManager interfaceManager;
    GameObject icon;

    RecipeMemory rm;

    private void Awake()
    {
        NIControl = GameObject.Find("HeadLockCanvas").GetComponent<NIThresholdControl>();
        buttonClip = GameObject.Find("Button_Click").GetComponent<AudioSource>();
        interfaceManager = GameObject.Find("InterfaceManager").GetComponent<InterfaceManager>();
        rm = GameObject.Find("RecipeMemory").GetComponent<RecipeMemory>();
        if (name == "BrowseRecipeButton" || name == "ScanMyIngredientsButton" || name == "WelcomeInterface") return;
        GameObject iconText = transform.parent.transform.Find("IconAndText").gameObject;
        icon = iconText.transform.Find("Icon").gameObject;
        
    }

    private IEnumerator ShowFeedback()
    {
        
        icon.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(1.0f);
        GetComponent<Collider>().enabled = true;
        
        icon.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hand" && interfaceManager.clickOk())
        {
            interfaceManager.clickButton();
            GetComponent<Button>().onClick.Invoke();
            StartCoroutine(ShowFeedback());
        }
    }

    public void clicked()
    {
        
        if (name == "ScanMyIngredientsButton")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("ScanMyIngredientsButton").transform.position);
            interfaceManager.setActiveNearInterface(false);
            interfaceManager.setActiveOnboardingInterface(false);
            interfaceManager.setActiveWelcomeInterface(false);
            interfaceManager.setActiveScanningInterface(true);

        }
        else if (name == "BrowseRecipeButton")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("BrowseRecipeButton").transform.position);
            interfaceManager.setActiveNearInterface(false);
            interfaceManager.setActiveScanningInterface(false);
            interfaceManager.setActiveWelcomeInterface(false);
            interfaceManager.setActiveOnboardingInterface(true);
            StartCoroutine(rm.updateOnboardingWithMemory());
            
        }
        else if (name == "Lock")
        {
            /*
            NIControl.changeLock();
            UpdateInGameInterface uii = GameObject.Find("InGameInterface").GetComponent<UpdateInGameInterface>();
            uii.updateLock(NIControl.getLock());
            AudioSource.PlayClipAtPoint(buttonClip.clip, GameObject.Find("Lock").transform.position);
            */
            NIControl.changeLock();
            AudioSource.PlayClipAtPoint(buttonClip.clip, this.transform.position);
        }
    }
}
