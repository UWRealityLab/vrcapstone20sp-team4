using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.Video;

public class VisualCueManager : MonoBehaviour
{
    // Start is called before the first frame update
    private VisualCueTask currTask;
    private ApplicationState appState;
    private VideoPlayer videoPlayer;
    private Dictionary<string, VideoClip> actionsCues = new Dictionary<string, VideoClip>();
    public GameObject equipmentIndicator;
    public GameObject ingredientIndicator;
    private float criticalEquipmentUpdateTimer = 0;
    public Dictionary<string, GameObject> nameToObject = new Dictionary<string, GameObject>();
    void Start()
    {
        appState = GameObject.Find("ApplicationState").GetComponent<ApplicationState>();
        videoPlayer = GameObject.Find("NearInterface/GameInterfaceScreen/VideoInterface").GetComponent<VideoPlayer>();
        actionsCues["cut"] = Resources.Load<VideoClip>("actions/cutting");
        actionsCues["crack"] = Resources.Load<VideoClip>("actions/egg_cracking");
        actionsCues["heat"] = Resources.Load<VideoClip>("actions/heating");
        actionsCues["melt"] = Resources.Load<VideoClip>("actions/melting");
        actionsCues["mix"] = Resources.Load<VideoClip>("actions/mixing");
        actionsCues["slice"] = Resources.Load<VideoClip>("actions/slicing");
        actionsCues["spread"] = Resources.Load<VideoClip>("actions/spread");
        actionsCues["sprinkle"] = Resources.Load<VideoClip>("actions/sprinkle");
    }

    // Update is called once per frame
    void Update()
    {
        if (currTask != null)
        {
            criticalEquipmentUpdateTimer = Mathf.Max(0, criticalEquipmentUpdateTimer - Time.deltaTime);
            if (criticalEquipmentUpdateTimer == 0)
            {

            }
        }
    }

    public void setTasks(VisualCueTask task)
    {
        // clean all state before setting a new task
        foreach (string name in nameToObject.Keys)
        {
            Destroy(nameToObject[name]);
        }
        nameToObject.Clear();
        criticalEquipmentUpdateTimer = 0;
        currTask = task;
    }

    public class VisualCueTask
    {
        public string action;
        public List<string> equipment;
        public List<string> ingredients;
    }

    public void DisplayVideo(string utensil, string action)
    {
        if (!actionsCues.ContainsKey(action)) {
            Debug.Log("No such action: " + action);
            return;
        }

        Vector3 loc = appState.GetLocation(name);
        if (loc == Vector3.zero) {
            Debug.Log("Cannot get location info for: " + utensil);
            return;
        }

        // set the location of videoInterface
        loc += new Vector3(loc.x, loc.y, loc.z + 5.0f);
        videoPlayer.transform.position = loc;

        // set the video clip and play
        VideoClip video = actionsCues[action];
        videoPlayer.clip = video;
        videoPlayer.Play();
    }

}
