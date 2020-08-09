using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is for keeping and updating important states info 
// that other program will want to acquire
public class ApplicationState : MonoBehaviour
{

    private Dictionary<string, Vector3> item2Location = new Dictionary<string, Vector3>();

    // in the future, we might need 
    // list of all current animation
    // more...
    public bool contains(string name)
    {
        return item2Location.ContainsKey(name);
    }

    // contains should be called before this
    public Vector3 getItemLocation(string name)
    {
        return item2Location[name];
    }

    public void setLocation(string name, Vector3 location)
    {
        item2Location[name] = location;
    }


}
