using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

// This class is for keeping and updating important states info 
// that other program will want to acquire
public class ApplicationState : MonoBehaviour
{

    private int timeStamp = 0;
    private List<string> singletonEquipments = new List<string>() { "microwave", "fork", "bowl", "spoon", "knife" };
    private Dictionary<string, Vector3> singleEquipMap = new Dictionary<string, Vector3>();
    void Start()
    {
    }




    public bool contains(string name)
    {
        return singleEquipMap.ContainsKey(name);
    }

    public void setLocation(string name, Vector3 position)
    {
        name = name.ToLower();
        // Debug.Log("set location for: " + name);
        if (singletonEquipments.Contains(name))
        {
            if (!singleEquipMap.ContainsKey(name)) {
                Debug.Log(name + " detected");
            }
            
            singleEquipMap[name] = position;
        }
        // Debug.Log("done setting location for: " + name + " : " + position.x + " " + position.y + " " + position.z);
    }

    public Vector3 GetLocation(string name)
    {
        if (singleEquipMap.ContainsKey(name)) {
            return singleEquipMap[name];
        } else {
            return Vector3.zero;
        }
    }

    public string getInfo() {
        string res = "";
        foreach (string s in singleEquipMap.Keys) {
            res += s + " ";
        }
        return res + "currently stored";
    }

    // public void Clear()
    // {
    //     singleEquipMap.Clear();
    // }

}
