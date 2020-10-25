using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonVisualizer : MonoBehaviour
{
    Material normalMat;
    Material highlightMat;

    private void Start()
    {
        normalMat = Resources.Load("Mat/BoxMat", typeof(Material)) as Material;
        highlightMat = Resources.Load("Mat/HighlightMat", typeof(Material)) as Material;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Hand"))
        {
            if (tag == "RecipePlate")
            {
                GameObject frontPlate = transform.Find("FrontPlate").gameObject;
                frontPlate.GetComponent<Renderer>().material = highlightMat;
            }
            else
            {
                transform.Find("FrontPlate").GetComponent<Renderer>().material = highlightMat;
            }
            //Debug.Log("OnTriggerEnter: ButtonViz");
            //frontPlate.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Hand"))
        {
            if (tag == "RecipePlate")
            {
                GameObject frontPlate = transform.Find("FrontPlate").gameObject;
                frontPlate.GetComponent<Renderer>().material = normalMat;
            }
            else
            {
                transform.Find("FrontPlate").GetComponent<Renderer>().material = normalMat;
                //frontPlate.SetActive(false);
            }
        }
    }

    void OnDisable() {
        transform.Find("FrontPlate").GetComponent<Renderer>().material = normalMat;
    }
}
