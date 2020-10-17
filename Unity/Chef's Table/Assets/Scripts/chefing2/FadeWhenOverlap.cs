using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeWhenOverlap : MonoBehaviour
{
    
    // fade the current go when overlapping visually with another one
    public GameObject another;

    // current game object's center
    public GameObject mainCam;

    private float timer = 1f;
    private bool wasOverlapping = false;

    void Update()
    {
        bool overlapping = isOverlapping();
        if (wasOverlapping != overlapping) {
            timer = 1;
        }
        timer = Mathf.Max(0, timer - Time.deltaTime);
        changeScale(overlapping);
        wasOverlapping = overlapping;
    }

    public bool isOverlapping() {
        Vector3 viewVector = mainCam.transform.forward;
        Vector3 toAnother = another.transform.position - mainCam.transform.position;
        // method 1: through dot product:
        float angle = Vector3.Angle(viewVector, toAnother);
        return angle < 25;
    }

    private void changeScale(bool overlapping) {
        Vector3 scale = this.transform.localScale;
        if (overlapping) {        
            scale.Set(timer * 1, timer * 1, timer * 1);
        } else {
            scale.Set((1 - timer) * 1, (1 - timer) * 1, (1 - timer) * 1);
        }
        Debug.Log(scale);
        this.transform.localScale = scale;
    }
}
