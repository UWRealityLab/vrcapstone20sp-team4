using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class KeyBoardButton : MonoBehaviour
{
    InterfaceManager interfaceManager;
    private GameObject target;
    private GameObject targetPlaceholder;

    private ScanningInterfaceController controller;

    private void Awake()
    {
        controller = GameObject.Find("ScanningContainer").GetComponent<ScanningInterfaceController>();
        GameObject.Find("Button_Click").GetComponent<AudioSource>();
        target = transform.parent.parent.Find("Input/Text").gameObject;
        targetPlaceholder = transform.parent.parent.Find("Input/Placeholder").gameObject;
        interfaceManager = GameObject.Find("InterfaceManager").GetComponent<InterfaceManager>();
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

    private IEnumerator ShowFeedback()
    {
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(1.0f);
        GetComponent<Collider>().enabled = true;
    }

    public void clicked()
    {
        //AudioSource.PlayClipAtPoint(buttonClip.clip, this.transform.position);
        if (name == "backspace")
        {
            try {
                TextMeshPro targetText = target.GetComponent<TextMeshPro>();
                if (targetText.text.Length > 0)
                {
                    targetText.text = targetText.text.Remove(targetText.text.Length - 1);
                }
                if (targetText.text.Length == 0)
                {
                    targetPlaceholder.SetActive(true);
                    target.SetActive(false);
                }
            } catch (NullReferenceException e)
            {
                targetPlaceholder.SetActive(true);
                target.SetActive(false);
            }
        }
        else if (name == "confirm") 
        {
            if (!target.activeSelf) {
                controller.updateIngredientListByInput(new List<string>());
            } else {
                List<string> res = new List<string>();
                string[] keyboardInputs = target.GetComponent<TextMeshPro>().text.Split(',');
                foreach (string s in keyboardInputs)
                {
                    string ss = s.Trim();
                    if (ss.Length > 0) res.Add(ss);
                }
                controller.updateIngredientListByInput(res);
            }
            // ewwwww, really bad style. Find a random instance of scanning interface button
            GameObject.Find("ScanningContainer/KeepInFront/Exit/ExitScript").GetComponent<ScanningInterfaceButton>().keyboardSwitchFunc(false);
        } 
        else
        {
            targetPlaceholder.SetActive(false);
            target.SetActive(true);
            string letter = name;
            TextMeshPro targetText = target.GetComponent<TextMeshPro>();
            targetText.text = targetText.text + letter;
        }

        
    }
}
