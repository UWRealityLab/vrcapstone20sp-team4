using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BurritoControlPointTrigger : MonoBehaviour
{
    private List<GameObject> inContact = new List<GameObject>();
    public List<GameObject> toIgnore;
    private Animator toritilla_anim;
    private int anim_state = 1; //1 ~ 4
    private int handle_num;

    // Start is called before the first frame update
    void Start()
    {
        handle_num = Int32.Parse(name.Substring(name.Length - 1));
        toritilla_anim = GameObject.Find("wrappingSimulation/tortilla").GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (inContact.Count > 0) {
            Vector3 accumulate = Vector3.zero;
            foreach (GameObject keyPoint in inContact)
            {
                accumulate += keyPoint.transform.position;
            }
            accumulate /= inContact.Count;
            gameObject.transform.position = accumulate;
        }
        var currentState = toritilla_anim.GetCurrentAnimatorStateInfo(0);
        if (currentState.fullPathHash == Animator.StringToHash("Base Layer.01"))
        {
            anim_state = 1;
        } else if (currentState.fullPathHash == Animator.StringToHash("Base Layer.02"))
        {
            anim_state = 2;
        }
        else if (currentState.fullPathHash == Animator.StringToHash("Base Layer.03"))
        {
            anim_state = 3;
        } else
        {
            anim_state = 4;
        }
    }

    public void claimKeyPointOwnerShip(GameObject keyPoint) // for other handle to call
    {
        if (inContact.Contains(keyPoint))
        {
            inContact.Remove(keyPoint);
        }
    }

    //When the Primitive exits the collision, it will change Color
    private void OnTriggerEnter(Collider other)
    {
        if (handle_num != anim_state)
        {
            inContact.Clear();
            return;
        }
        if (!toIgnore.Contains(other.gameObject))
        {
            inContact.Add(other.gameObject);
            //foreach (GameObject handle in toIgnore)
            //{
            //    handle.GetComponent<BurritoControlPointTrigger>().claimKeyPointOwnerShip(other.gameObject);
            //}
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (!toIgnore.Contains(other.gameObject) && inContact.Contains(other.gameObject))
        {
            inContact.Remove(other.gameObject);
        }
    }

}
