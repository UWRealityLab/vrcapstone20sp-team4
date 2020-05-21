using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonVisualizer : MonoBehaviour
{
    Material normalMat;
    Material highlightMat;
    private GameObject frontPlate;

    private void Start()
    {
        frontPlate = transform.Find("FrontPlate").gameObject;
        normalMat = Resources.Load("Mat/BoxMat", typeof(Material)) as Material;
        highlightMat = Resources.Load("Mat/HighlightMat", typeof(Material)) as Material;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Hand"))
        {
            GetComponent<Renderer>().material = highlightMat;
            //Debug.Log("OnTriggerEnter: ButtonViz");
            //frontPlate.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Hand"))
        {
            GetComponent<Renderer>().material = normalMat;
            //frontPlate.SetActive(false);
        }
    }
}
