using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine;

public class changeSimulation : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject slicedObjects;
    public List<GameObject> prefabs;
    public List<VideoClip> videos; // indice must match with prefabs
    public GameObject mainCam;
    private int index;
    private GameObject videoScreen;
    private VideoPlayer vp;

    private void Start()
    {
        slicedObjects = GameObject.Find("CuttingSimulation/SlicedObject");
        videoScreen = GameObject.Find("SimulationVideoScreen");
        vp = videoScreen.transform.Find("Screen").GetComponent<VideoPlayer>();
        
        index = 0;
    }

    void Update()
    {
        videoScreen.transform.LookAt(mainCam.transform.position);
        
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
        vp.clip = videos[index]; // update video played
        foreach (Transform child in slicedObjects.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        GameObject go = (GameObject)Instantiate(prefab, slicedObjects.transform.position, slicedObjects.transform.rotation);
        go.transform.SetParent(slicedObjects.transform);
    }
}

