using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

// This class stores the object tracking information for the tutorial

public class ApplicationState : MonoBehaviour
{

    private int timeStamp = 0;
    private List<string> singletonEquipments = new List<string>() { "microwave", "fork", "bowl", "spoon", "knife" };
    private Dictionary<string, Vector3> singleEquipMap = new Dictionary<string, Vector3>();
    void Start()
    {
    }

    // contains an object
    public bool contains(string name)
    {
        return singleEquipMap.ContainsKey(name);
    }

    // set an object location
    public void setLocation(string name, Vector3 position)
    {
        name = name.ToLower();
        if (singletonEquipments.Contains(name))
        {
            if (!singleEquipMap.ContainsKey(name)) {
                Debug.Log(name + " detected");
            }
            
            singleEquipMap[name] = position;
        }
    }

    // look up an object
    public Vector3 GetLocation(string name)
    {
        if (singleEquipMap.ContainsKey(name)) {
            return singleEquipMap[name];
        } else {
            return Vector3.zero;
        }
    }

    // a helper function similar to toString()
    public string getInfo() {
        string res = "";
        foreach (string s in singleEquipMap.Keys) {
            res += s + " ";
        }
        return res + "currently stored";
    }


}
