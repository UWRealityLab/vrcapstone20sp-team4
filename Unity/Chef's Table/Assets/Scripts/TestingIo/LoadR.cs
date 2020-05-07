using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadR : MonoBehaviour
{
    // Start is called before the first frame update
   // public GameObject resouceLoad;
    void Start()
    {
        GameObject animation = (GameObject)Instantiate(Resources.Load("Animations/breakfast burrito/step8"), new Vector3(0, 0, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
