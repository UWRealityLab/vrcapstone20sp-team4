using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfCleanUp : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        int children = transform.childCount;
        if (children > 25)
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }
}
