
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
    public GameObject scanningActive;
    private bool networkErrorOccured = false;
    private bool isPaused = false;
    private int currentNotification = 0;
    private int lastNotification = 0;

    public string[] array()
    {
        if (virtualKeyboardText.activeSelf)
        {
            string[] keyboardInputs = virtualKeyboardText.GetComponent<TextMeshPro>().text.Split(',');
            foreach (string s in keyboardInputs)
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
            GameObject textObject = go.transform.Find("IngredientNameText").Find("IngredientName").gameObject;
            ignore.Add(textObject.GetComponent<TextMeshPro>().text);
            currentLabels.Remove(textObject.GetComponent<TextMeshPro>().text);
            textObject.GetComponent<TextMeshPro>().text = "none";
            go.SetActive(false);
        }
        lastNotification--;
        if (lastNotification == 6)
        {
            // remove max notification warning
            lastNotification--;
        }
        handleRender();
    }

    public void clearMemory()
    {
        for (int i = 0; i < 6; i++)
        {
            removeIngredient(i);
        }
        currentNotification = 0;
        lastNotification = 0;
        ignore.Clear();
        currentLabels.Clear();
    }

    public void handleRender()
    {
        if (networkErrorOccured)
        {
            statusPanel.SetActive(true);
            statusPanel.transform.Find("ErrorMessage").gameObject.SetActive(true);
            statusPanel.transform.Find("loading").gameObject.SetActive(false);
        }
        else
        {
            if (currentLabels.Count == 0)
            {
                statusPanel.SetActive(true);
                statusPanel.transform.Find("ErrorMessage").gameObject.SetActive(false);
                statusPanel.transform.Find("loading").gameObject.SetActive(true);
            }
            else
            {
                statusPanel.SetActive(false);
            }
        }
    }

    public void handleResponseStatus()
    {
        if (networkErrorOccured)
        {
            clearMemory();
        }
        handleRender();
    }

    public void pause()
    {
        isPaused = true;
    }

    public void play()
    {
        isPaused = false;
    }

    void Update()
    {
        // Notification Label Management
        if (scanningActive.activeSelf &&
            scanningActive.transform.Find("Notification " + currentNotification).gameObject.activeSelf &&
            scanningActive.transform.Find("Notification " + currentNotification).
            transform.Find("BackPlate").GetComponent<MeshRenderer>().material.color.a < 0f)
        {
            for (int i = currentNotification; i < 7; i++)
            {
                if (scanningActive.transform.Find("Notification " + i).gameObject.activeSelf)
                {
                    //Debug.Log("position untransformed " + i + " " + scanningActive.transform.Find("Notification " + i).transform.localPosition.y);
                    scanningActive.transform.Find("Notification " + i).transform.localPosition = new Vector3(
                        scanningActive.transform.Find("Notification " + i).transform.localPosition.x,
                        scanningActive.transform.Find("Notification " + i).transform.localPosition.y + 0.015f,
                        scanningActive.transform.Find("Notification " + i).transform.localPosition.z);
                    //Debug.Log("position " + i + " " + scanningActive.transform.Find("Notification " + i).transform.localPosition.y);
                }
            }
            scanningActive.transform.Find("Notification " + currentNotification).gameObject.SetActive(false);
            currentNotification++;
        }

        if (scanningActive.transform.Find("Notification " + currentNotification).gameObject.activeSelf)
        {
            scanningActive.transform.Find("Notification " + currentNotification).transform.Find("IconAndText").Find("Text").gameObject.GetComponent<TextMeshPro>().alpha -= 0.03f;
            Color backPlate = scanningActive.transform.Find("Notification " + currentNotification).transform.Find("BackPlate").gameObject.GetComponent<MeshRenderer>().material.color;
            Color icon = scanningActive.transform.Find("Notification " + currentNotification).transform.Find("IconAndText").Find("Icon").gameObject.GetComponent<MeshRenderer>().material.color; 
            icon.a -= 0.03f;
            backPlate.a -= 0.03f;
            scanningActive.transform.Find("Notification " + currentNotification).transform.Find("IconAndText").Find("Icon").gameObject.GetComponent<MeshRenderer>().material.color = icon;
            scanningActive.transform.Find("Notification " + currentNotification).transform.Find("BackPlate").gameObject.GetComponent<MeshRenderer>().material.color = backPlate;
        }

    }

    public void updateIngredientList(string response)
    {
        if (response.Equals("error"))
        {
            networkErrorOccured = true;
            handleResponseStatus();
            return;
        } else if (isPaused)
        {
            return;
        }
        else
        {
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
                        go.transform.Find("IngredientNameText").Find("IngredientName").gameObject.GetComponent<TextMeshPro>().text = label;

                        GameObject notification = GameObject.Find("Notification " + lastNotification);
                        notification.SetActive(true);
                        notification.gameObject.GetComponent<TextMeshPro>().text = label + " detected";
                        lastNotification++;
                        if (limit == 6)
                        {
                            GameObject maxNotification = GameObject.Find("Notification " + lastNotification);
                            maxNotification.SetActive(true);
                        }

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