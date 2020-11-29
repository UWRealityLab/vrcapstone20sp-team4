using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

// union of helper method that enables manipulation of the scanning interface
public class ScanningInterfaceController : MonoBehaviour
{
    private HashSet<string> ignore = new HashSet<string>();
    public List<GameObject> ingredients = new List<GameObject>();
    private HashSet<string> currentLabels = new HashSet<string>();
    private Queue<GameObject> notificationQueue = new Queue<GameObject>();
    public GameObject notificationPrefab;
    public GameObject notifications;
    private bool isPaused = false;
    private int notificationIndex = 0;
    private float notificationOffset = -0.02f;

    private void Awake() {
        foreach (GameObject go in ingredients) {
            if (go.activeSelf) {
                go.SetActive(false);
            }
        }
    }

    // return current ingredient
    public string[] array()
    {
        String[] stringArray = new String[currentLabels.Count];
        currentLabels.CopyTo(stringArray);
        return stringArray;
    }

    private void Update()
    {
        if (notificationQueue.Count == 0)
        {
            notificationIndex = 0;
        }
    }

    // create an notification on the interface when an action is made
    private void createNotifications(string text)
    {
        GameObject notificationContainer = Instantiate(notificationPrefab, Vector3.zero, Quaternion.identity);
        notificationContainer.transform.Find("Notification").Find("IconAndText").Find("Text").gameObject.GetComponent<TextMeshPro>().text = text;
        notificationQueue.Enqueue(notificationContainer);
        notificationContainer.transform.parent = notifications.transform;
        notificationContainer.transform.localPosition = Vector3.zero + new Vector3(0, notificationIndex * notificationOffset, 0);
        notificationContainer.transform.localRotation = Quaternion.identity;
        notificationIndex += 1;
        StartCoroutine("registerDisposer");
    }

    // destroy a notification
    private IEnumerator registerDisposer()
    {
        yield return new WaitForSeconds(4.01f);
        GameObject notificationContainer = notificationQueue.Dequeue();
        Destroy(notificationContainer);
    }

    // delete an existing ingredient
    public void removeIngredient(int index)
    {
        GameObject go = ingredients[index];
        if (go.activeSelf)
        {
            GameObject textObject = go.transform.Find("name").gameObject;
            string name = textObject.GetComponent<TextMeshPro>().text;
            ignore.Add(name);
            currentLabels.Remove(textObject.GetComponent<TextMeshPro>().text);
            textObject.GetComponent<TextMeshPro>().text = "none";
            go.SetActive(false);
            createNotifications(name + " removed");
        }
    }

    // reset state
    public void clearMemory()
    {
        for (int i = 0; i < 6; i++)
        {
            removeIngredient(i);
        }
        ignore.Clear();
        currentLabels.Clear();
    }

    // load icons from the database
    private Sprite loadIngredientIcon(string name) {
        Sprite icon = Resources.Load<Sprite>("Image/IngredientIcons/" + name);
        if (icon == null) {
            icon = Resources.Load<Sprite>("Image/IngredientIcons/default");
        }
        return icon;
    }

    // add an ingredient
    public void addIngredient(List<string> ingredientsName)
    {
        int limit = Math.Min(6, ingredientsName.Count);
        for (int i = 0; i < limit; i++)
        {
            string label = ingredientsName[i].ToLower();
            if (currentLabels.Contains(label))
            {
                createNotifications(label + " already exists");
                continue;
            }
            else
            {
                createNotifications(label + " added");
            }

            if (!ignore.Contains(label))
            {
                for (int j = 0; j < 6; j++)
                {
                    GameObject go = ingredients[j];
                    if (!go.activeSelf)
                    {
                        go.SetActive(true);
                        go.transform.Find("name").gameObject.GetComponent<TextMeshPro>().text = label;
                        go.transform.Find("mask").Find("IngredientImage").gameObject.GetComponent<Image>().sprite = loadIngredientIcon(label.ToLower());
                        currentLabels.Add(label);
                        break;
                    }
                }
            }
        }
        for (int i = limit; i < ingredientsName.Count; i++) {
            string label = ingredientsName[i];
            createNotifications("fail to add " + label);
        }
    }

    public void pause()
    {
        isPaused = true;
    }

    public void play()
    {
        isPaused = false;
    }

    // add ingredient through virtual keyboard input
    public void updateIngredientListByInput(List<string> ingredientsName)
    {
        addIngredient(ingredientsName);
    }

    // add ingredient through detection pipeline
    public void updateIngredientListByScanning(string response)
    {
        string res = "{\"detections\": " + response + " }";
        Debug.Log(res);
        ListOfDetections listOfDetections;
        try
        {
            listOfDetections = JsonUtility.FromJson<ListOfDetections>(res);
        }
        catch (Exception e)
        {
            return;
        }
        addIngredient(listOfDetections.detections);
    }

    // helper class for parsing the pipeline response
    [Serializable]
    public class ListOfDetections
    {
        public List<string> detections;
    }
    // this script controls rendering of the scanning interface

}