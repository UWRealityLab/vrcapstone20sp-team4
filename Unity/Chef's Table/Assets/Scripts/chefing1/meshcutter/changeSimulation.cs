using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine;

public class changeSimulation : MonoBehaviour
{
    // Start is called before the first frame update
    // split into mode cut and stir
    private string mode = "cut"; // cut or skillet

    // cutting mode 
    public GameObject cuttingSimulation;


    // skillet mode
    public GameObject skilletSimulation;

    // tortilla mode 
    public GameObject tortillaSimulation;
    // common
    public GameObject mainCam;
    public GameObject addIngredientButton;
    private int index;
    private GameObject videoScreen;
    private VideoPlayer vp;
    private List<GameObject> active_prefabs;
    private List<VideoClip> active_videos;
    public List<GameObject> cut_prefabs;
    public List<VideoClip> cut_videos; // indice must match with prefabs
    public List<GameObject> ingredient_prefabs;
    public List<VideoClip> skillet_videos;

    private void Awake()
    {
        videoScreen = GameObject.Find("SimulationVideoScreen");
        vp = videoScreen.transform.Find("Screen").GetComponent<VideoPlayer>();
    }

    void Start()
    {
        index = 0;

    }

    void Update()
    {
        videoScreen.transform.LookAt(mainCam.transform.position);
    }

    void OnEnable()
    {
        resetMode();
    }
    public void resetMode()
    {
        index = 0;
        if (mode == "tortilla")
        {
            active_prefabs = cut_prefabs;
            active_videos = cut_videos;
            cuttingSimulation.SetActive(true);
            skilletSimulation.SetActive(false);
            tortillaSimulation.SetActive(false);
            videoScreen.SetActive(true);
            vp.clip = active_videos[index];
            mode = "cut";
        } else if (mode == "skillet")
        {
            active_prefabs = null;
            active_videos = null;
            cuttingSimulation.SetActive(false);
            skilletSimulation.SetActive(false);
            tortillaSimulation.SetActive(true);
            videoScreen.SetActive(false);
            mode = "tortilla";
        } else if (mode == "cut") {
            active_prefabs = ingredient_prefabs;
            active_videos = skillet_videos;
            cuttingSimulation.SetActive(false);
            skilletSimulation.SetActive(true);
            tortillaSimulation.SetActive(false);
            videoScreen.SetActive(true);
            vp.clip = active_videos[index];
            mode = "skillet";
        }
    }

    public void nextObject()
    {
        index++;
        if (index == active_prefabs.Count)
        {
            index = 0;
        }
        updateObject(active_prefabs[index]);
    }

    public void previousObject()
    {
        index--;
        if (index == -1)
        {
            index = active_prefabs.Count - 1;
        }
        updateObject(active_prefabs[index]);
    }

    public void changeObject()
    {
        index++;
        index = index % active_prefabs.Count;
        updateObject(active_prefabs[index]);
    }

    public void resetObject()
    {
        if (mode == "tortilla")
        {
            tortillaSimulation.transform.Find("tortilla").GetComponent<TortillaFolding>().reset();
        } else if (mode == "skillet")
        {
            GameObject spawn = GameObject.Find("CuttingSimulation/skilletSimulation/Skillet/spawn");
            foreach (Transform child in spawn.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        } else
        {
            updateObject(active_prefabs[index]);
        }
        
    }

    public void addIngredients()
    {
        if (mode == "skillet")
        {
            GameObject spawn = GameObject.Find("CuttingSimulation/skilletSimulation/Skillet/spawn");
            System.Random r = new System.Random();
            int range = r.Next(2, 4);
            
            for (int i = 0; i < range; i++)
            {
                float local_offset = (float) (r.NextDouble() * 0.2 -  0.1);
                Vector3 rv = new Vector3(local_offset, local_offset, local_offset);
                GameObject go = (GameObject)Instantiate(active_prefabs[index], spawn.transform.position + rv, spawn.transform.rotation);
                go.transform.SetParent(spawn.transform);
            }
            
        }
    }

    private void updateObject(GameObject prefab)
    {
        if (mode == "tortilla")
        {
            return;
        }
        if (index >=0 && index < active_videos.Count)
        {
            vp.clip = active_videos[index]; // update video played
        }
        
        if (mode == "cut")
        {
             GameObject slicedObjects = GameObject.Find("CuttingSimulation/cuttingSimulation/SlicedObject");
            foreach (Transform child in slicedObjects.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            GameObject go = (GameObject)Instantiate(prefab, slicedObjects.transform.position, new Quaternion(90, 90, 0, 0));
            go.transform.SetParent(slicedObjects.transform);
        } 
        // for skillet simulation, updates are done in reset and addingredients

    }


}

