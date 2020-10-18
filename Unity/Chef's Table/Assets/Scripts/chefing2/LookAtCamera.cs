using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject mainCamera;
    void Awake()
    {
        mainCamera = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCamera != null) {
            gameObject.transform.LookAt(mainCamera.transform.position);
        }
    }
}
