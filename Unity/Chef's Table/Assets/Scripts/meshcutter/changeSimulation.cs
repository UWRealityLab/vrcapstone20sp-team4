using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeSimulation : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject slicedObjects;
    public List<GameObject> prefabs;
    private int index;

    private void Start()
    {
        slicedObjects = GameObject.Find("CuttingSimulation/SlicedObject");
        index = 0;
    }

    public void nextObject()
    {
        index++;
        if (index == prefabs.Count)
        {
            index = 0;
        }
        updateObject(prefabs[index]);
    }

    public void previousObject()
    {
        index--;
        if (index == -1)
        {
            index = prefabs.Count - 1;
        }
        updateObject(prefabs[index]);
    }

    public void resetObject()
    {
        updateObject(prefabs[index]);
    }

    private void updateObject(GameObject prefab)
    {
        foreach (Transform child in slicedObjects.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        GameObject go = (GameObject)Instantiate(prefab, slicedObjects.transform.position, slicedObjects.transform.rotation);
        go.transform.SetParent(slicedObjects.transform);
    }
}

