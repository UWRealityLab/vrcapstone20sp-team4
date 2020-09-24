using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class ScanningInterfaceController : MonoBehaviour
{
    private HashSet<string> ignore = new HashSet<string>();
    public List<GameObject> ingredients = new List<GameObject>();
    private HashSet<string> currentLabels = new HashSet<string>();
    public GameObject statusPanel;
    public GameObject virtualKeyboardText;
    private bool networkErrorOccured = false;


    public string[] array()
    {
        if (virtualKeyboardText.activeSelf)
        {
            string[] keyboardInputs = virtualKeyboardText.GetComponent<TextMeshPro>().text.Split(',');
            foreach(string s in keyboardInputs)
            {
                string ss = s.Trim();
                if (ss.Length > 0) currentLabels.Add(ss);
            }
        }
        String[] stringArray = new String[currentLabels.Count];
        currentLabels.CopyTo(stringArray);
        return stringArray;
    }

    public void removeIngredient(int index)
    {
        GameObject go = ingredients[index];
        if (go.activeSelf)
        {
            GameObject textObject = go.transform.FindChild("IngredientNameText").FindChild("IngredientName").gameObject;
            ignore.Add(textObject.GetComponent<TextMeshPro>().text);
            currentLabels.Remove(textObject.GetComponent<TextMeshPro>().text);
            textObject.GetComponent<TextMeshPro>().text = "none";
            go.SetActive(false);
            
        }
        handleRender();
    }

    public void clearMemory()
    {
        for (int i = 0; i < 6; i++)
        {
            removeIngredient(i);
        }
        ignore.Clear();
        currentLabels.Clear();
    }

    public void handleRender()
    {
        if (networkErrorOccured) {
            statusPanel.SetActive(true);
            statusPanel.transform.FindChild("ErrorMessage").gameObject.SetActive(true);
            statusPanel.transform.FindChild("loading").gameObject.SetActive(false);
        } else {
            if (currentLabels.Count == 0) {
                statusPanel.SetActive(true);
                statusPanel.transform.FindChild("ErrorMessage").gameObject.SetActive(false);
                statusPanel.transform.FindChild("loading").gameObject.SetActive(true);
            } else {
                statusPanel.SetActive(false);
            }
        }
    }

    public void handleResponseStatus() {
        if (networkErrorOccured) {
            clearMemory();
        }
        handleRender();
    }

    public void updateIngredientList(string response)
    {
        if (response.Equals("error")) {
            networkErrorOccured = true;
            handleResponseStatus();
            return;
        } else {
            networkErrorOccured = false;
        }
        
        string res = "{\"detections\": " + response + " }";
        ListOfDetections listOfDetections;
        try
        {
            listOfDetections = JsonUtility.FromJson<ListOfDetections>(res);
        }
        catch (Exception e)
        {
            return;
        }

        int limit = Math.Min(6, listOfDetections.detections.Count);
        Debug.Log("limit = " + limit);
        for (int i = 0; i < limit; i++)
        {
            string label = listOfDetections.detections[i];
            if (currentLabels.Contains(label)) continue;
            if (!ignore.Contains(label))
            {
                for (int j = 0; j < 6; j++)
                {
                    GameObject go = ingredients[j];
                    
                    if (!go.activeSelf)
                    {
                        go.SetActive(true);
                        go.transform.FindChild("IngredientNameText").FindChild("IngredientName").gameObject.GetComponent<TextMeshPro>().text = label;
                        currentLabels.Add(label);
                        break;
                    }
                }
            }
        }
        handleResponseStatus();
    }

    [Serializable]
    public class ListOfDetections
    {
        public List<string> detections;
    }


    // this script controls rendering of the scanning interface

}
