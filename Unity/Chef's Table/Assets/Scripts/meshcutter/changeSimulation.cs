using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeSimulation : MonoBehaviour
{
    // Start is called before the first frame update
    public Object prefab;
    public GameObject SlicedObjects;
    
    // Update is called once per frame
    void clicked()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        GameObject go = (GameObject)Instantiate(prefab, SlicedObjects.transform.position, SlicedObjects.transform.rotation);
        go.transform.SetParent(SlicedObjects.transform);
        
    }
}

